﻿module OpeningIrfanView

open System.IO

open Errors
open Helpers
open Records
open ClosingFunctions
open Settings.MySettings
open ROP_Functions.MyFunctions

//******* DEFINITIONS OF FREQUENTLY CALLED FUNCTIONS       
//function 1
let private stringChoice x = MyString.getString((rcO.numberOfScannedFileDigits - Seq.length (x |> string)), rcO.stringZero)
let private stringChoicePA (x: string) = MyString.getString((rcO.numberOfScannedFileDigits - Seq.length (x |> string)), rcO.stringZero)

//function 2
(*
let private myKey =  
    let key x y = 
        sprintf "%s%s%s" 
        <| rcO.prefix
        <| string x 
        <| string y  
    stringChoice >> key //dosazovani odleva
*)

//z vyukovych duvodu - tahle fce potrebuje string, myKey potrebuje int
let private myKeyPA =  
    let keyPA = sprintf "%s%s%s" rcO.prefix 
    stringChoicePA >> keyPA 

//******* DEFINITIONS OF TWO SUBMAIN FUNCTIONS
//1
//impure kvuli vystupu na console
let private getLists low high (myMap: Map<string, int>) = 

    let getOption i =        
        let aux = (low, high) ||> MakingWondersWithAux.getAux 
        (<) (i + 1) aux  
        |> function         
            | true  -> //myKeyPA pouzita z vyukovych duvodu   
                     myMap
                     |> Map.tryFind (myKeyPA <| string (low + 1) <| string (low + (i + 1)))   
                     |> Option.bind (fun value -> Some (value, (i + 1))) //Tohle iteruje List.unfold getValue (-1)  
            | false -> 
                     None     
    
    (*
    let getOption i =        
        //see the code above
        match cond with  //myKeyPA pouzita z vyukovych duvodu
        | true  -> 
                   myMap |> Map.tryFind (myKeyPA <| string (low + 1) <| string (low + (i + 1))) //Map.tryFind je funkce //Vyraz musi byt zavisly na i   
                   |> function                                                     
                      | Some value -> Some (value, (i + 1))                                                        
                      | None       -> None    
        | false -> None     
    *)

    let numberOfFilesList1 = List.unfold getOption (-1)               //unfold to ping from Some till bumping into None :-)
                                                                     //List.unfold potrebuje option, vraci list hodnot vybranych z nejake podminky      
                       
    //Alternative code based on Brian Berns' answer to my question https://stackoverflow.com/questions/67267040/populating-immutable-lists-in-a-cycle
    let numberOfFilesList2 =       
        [ -1 .. myMap.Count - 1 ] 
        |> List.collect
            (fun i ->                                         
                    [
                        let cond = 
                            let aux = (low, high) ||> MakingWondersWithAux.getAux 
                            (<) (i + 1) aux       
                        match cond with  
                        | true  -> 
                                 match myMap = Map.empty with
                                 | true  -> 
                                          ()
                                 | false -> 
                                          match myMap |> Map.tryFind (myKeyPA <| string (low + 1) <| string (low + (i + 1))) with
                                          | Some value -> yield value
                                          | None       -> ()   
                        | false -> 
                                 ()
                    ]                                                 
            ) 

    do printfn "numberOfFilesList2 %A \n" <| numberOfFilesList2      
    do printfn "numberOfFilesList1 %A \n" <| numberOfFilesList1
                                               
    let endFilesToOpenList = numberOfFilesList2 |> List.scan (+) 0 |> List.skip 1
                                               
    do printfn "endFilesToOpenList %A \n" <| endFilesToOpenList
    numberOfFilesList2, endFilesToOpenList        

//2
//impure kvuli vystupu na console a spusteni IrfanView
let private showLastScannedFiles (listOfFiles: string list) ((numberOfFilesList, endFilesToOpenList): int list * int list) low =  //NEVYDEDUKOVAL :-)
      
    let listOfFilesLength = listOfFiles.Length
    
    endFilesToOpenList 
    |> List.iteri
          (fun i item -> 
                       let pathWithArgument() = 
                           match (item - 1) >= listOfFilesLength with
                           | true  ->                                
                                    let str = 
                                       sprintf"%s %s"         
                                       <| "\nNásledující počet skenů v excel. tabulce je vyšší než realita o hodnotu"
                                       <| string (item - listOfFilesLength)
                                    do printfn "%s" <| str
                                    let path = string <| listOfFiles.Item (listOfFilesLength - 1) 
                                    path 
                           | false -> 
                                    let path = string <| listOfFiles.Item (item - 1)
                                    path                                    
                        
                       let pathWithArgument = pathWithArgument()
                 
                       let startIrfanView x = System.Diagnostics.Process.Start(rcO.path2, pathWithArgument)  //spousteni IrfanView
                 
                       //let finalAction x = ()      
                       let result = //tohle zachyti neexistujici IrfanView, neni treba delat komplikovany pattern matching s fileInfo
                           let ropResults = tryWith startIrfanView (fun x -> ()) (fun ex -> ())                                                     
                           ropResults |> deconstructor1 error0  
                       result
                                     
                       let printIt() = 
                           let lastXyCharacters = pathWithArgument.Substring((pathWithArgument.Length - rcO.suffixAndExtLength), rcO.suffixAndExtLength)
                           let str = 
                               sprintf"%s%s [%s] %s %s" 
                               <| rcO.prefix
                               <| string (i + low) 
                               <| string (numberOfFilesList.Item i) 
                               <| "Press ENTER to continue (Esc for IrView closure)" 
                               <| lastXyCharacters                                    
                           do printfn "%s" str   
                       printIt()       
                        
                       do Process.browseThroughScans()
          )     

//******* MAIN FUNCTION DEFINITION - OPENING IRFANVIEW WITH LAST FILES IN THEIR RESPECTIVE FOLDERS => vystupni funkce

let private (>>=) condition nextFunc = //symbol “»=” is the standard way of writing bind as an infix operator.
    match condition with
    | false -> rc.imageViewerProcess |> getRidOfItAll  
    | true  -> nextFunc() 

type private MyPatternBuilder = MyPatternBuilder with            
    member _.Bind(condition, nextFunc) = (>>=) <| condition <| nextFunc 
    member _.Return x = x

//zkouska alternativniho reseni s class a parametrem     
type private MyPatternBuilder1 (param: Lazy<int>) =             
    member _.Bind(condition, nextFunc) = 
        match condition with
        | false -> param.Force()
        | true  -> nextFunc()      
    member _.Return x = x
    
let openIrfanView param =  
    
     let (myRecordParams, createdList) = param

     MyPatternBuilder1 (lazy (rc.imageViewerProcess |> getRidOfItAll))     
         {   
            let low = myRecordParams.low |> function Low value -> value                
            let high = myRecordParams.high |> function High value -> value
            let myMap = myRecordParams.myMap |> function MyMap value -> value 

            let! _ = (<>) createdList List.Empty            
            let! _ = Seq.length <| myKeyPA (string low) (string low) = (+) (rcO.prefix |> Seq.length) rcO.numberOfScannedFileDigits                  
            let! _ = myMap |> Map.containsKey (myKeyPA (string low) (string low)) //argumenty fce su v takovem poradi: Map.containsKey key table, takze bez |> bude Map.containsKey (myKey low low) myMap          

            let showScans = 
                let getLists() = getLists low high myMap
                do showLastScannedFiles <| createdList <| getLists() <| low                   
                0
                //closeApp <| rc.imageViewerProcess //nahrazeno rekurzivni funkci pro opakovani programu, a to aji s nekterymi exceptions             
            return showScans
         }