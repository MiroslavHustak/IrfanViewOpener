module GettingInputValues

open System
open System.IO;
open System.Data
open Newtonsoft.Json
open System.Runtime.Serialization.Formatters.Binary

open Errors
open Records
open Helpers
open Settings
open DiscriminatedUnions
open Settings.MySettings
open ROP_Functions.MyFunctions

//******* DEFINITIONS OF FREQUENTLY CALLED FUNCTIONS ********************       
//function 1
let private stringChoice x = MyString.getString((rcO.numberOfScannedFileDigits - Seq.length (x |> string)), rcO.stringZero)

//******* MAIN FUNCTION (tady je jen jedna) ****************************
//nested functions, vstup data typu datatable option, vystup jsou opet data, tentokrat tuple typu record * ... (partial application)
//impure (logicky, stale bereme data z okolniho "impure" prostredi)

let getInputValues readFromExcel =  
    
    //first submain function
    let firstInputDataGroup = 

        let createMyMap (dtXlsx: DataTable) =  //compiler nedokazal dedukovat typ DataTable u parametru         
            let dtXlsxRows = dtXlsx.Rows      
            let myIntNumber i = 
                let columnNumber2 = rc.columnIndex |> Array.last //sloupec P 
                let aux = dtXlsxRows.[i].[columnNumber2] 
                          |> string 
                          |> Option.ofObj            
                Parsing1.parseMe1(optionToString aux)
            let myStringOP i = 
                let columnNumber1 = rc.columnIndex |> Array.head //sloupec A
                let aux = dtXlsxRows.[i].[columnNumber1] 
                          |> string 
                          |> Option.ofObj 
                optionToString aux
            let myMap: Map<string, int> = //neco na zpusob Dictionary<key, value>, key stejne jako v C# nemusi byt int
                let dtXlsxRowsCount = dtXlsxRows.Count                       
                let listRange = [ 0 .. dtXlsxRowsCount - 1 ]
                let rec loop list acc i =   //viz podrobna verze IrfanViewOpener - pro porovnani je tam tvorba Map pomoci seq nebo array nebo list
                    match list with 
                    | []        -> acc
                    | _ :: tail -> let checkRows = 
                                       let cond = not ((myStringOP i).Contains(rcO.prefix)) 
                                       match cond with
                                       | false -> let finalMap = Map.add (myStringOP i) (myIntNumber i) acc
                                                  loop tail finalMap (i + 1) //Tail-recursive function calls that have their parameters passed by the pipe operator are not optimized as loops #6984
                                       | true  -> loop tail acc (i + 1) //Tail-recursive function calls that have their parameters passed by the pipe operator are not optimized as loops #6984                                             
                                   checkRows   
                loop listRange Map.empty 0
            myMap    
         
        let getMyMap() =
            match readFromExcel with 
            | Some value -> value |> createMyMap                                           
            | None       -> do error9() 
                            Map.empty    

        let getInputValuesUI() =  
            do printfn "Prosím zadej vstupní hodnoty" 
            let p name again = 
                do printfn "Zadej %s číselnou část LT %s" 
                   <| name 
                   <| again                
                Parsing.parseMe(Console.ReadLine())          
   
            let fullParse() =         
                let parseHighLow again = struct (p "levou" again, p "pravou" again)                   
                let rec verify struct (low, high) =                    
                    match high > low with
                    | true  -> struct (low, high) 
                    | false -> verify ("ještě jednou" |> parseHighLow)                 
                do Console.Clear()
                verify (String.Empty |> parseHighLow )
            
            let struct (low, high) = fullParse()
            low, high       
    
        //Async for learning purposes only - async jinak vubec neni vhodny v console app, neb se prekryvaji chybova hlaseni  
        let myTaskFunctionDU x =    
            let task3 param = async { return TupleIntInt param }
            let task4 param = async { return MapStringInt param }  
            
            let du: TaskResults list = [
                                          task3 (getInputValuesUI())
                                          task4 (getMyMap())
                                       ] 
                                       |> Async.Parallel 
                                       |> Async.Catch
                                       |> Async.RunSynchronously
                                       |> function
                                           | Choice1Of2 result    -> result |> List.ofArray
                                           | Choice2Of2 (ex: exn) -> do printfn "Popis chyby async 2: %s" <| ex.Message
                                                                     do error11()
                                                                     List.empty  

            let whatIs(x: obj) =  //primy dynamic cast :?> TaskResults muze vest k chybe behem runtime
                match x with
                | :? TaskResults as du -> du  //aby nedoslo k chybe behem runtime
                | _                    -> do error16 "error16 - x :?> TaskResults"                                      
                                          x :?> TaskResults
        
            let inputValues = 
                let du = du |> List.head |> whatIs 
                match du with 
                | TupleIntInt(low, high) -> (low, high)                                                    
                | _                      -> do error16 "error16 - TupleIntInt" 
                                            (-1, -1) //whatever of the particular type
          
            let myMap =                 
                let du = du |> List.last |> whatIs 
                match du with 
                | MapStringInt value -> value                                           
                | _                  -> do error16 "error16 - MapStringInt" 
                                        Map.empty                                             
            inputValues, myMap 
        
        let rcInitValDu = 
            let resultDu = 
               let ropResults() = tryWith myTaskFunctionDU (fun x -> ()) (fun ex -> ()) 
               ropResults() |> deconstructor 
       
            {  
                low   = Low (fst (fst resultDu))
                high  = High (snd (fst resultDu))
                myMap = MyMap (snd resultDu)   
            }                  
        rcInitValDu    
    
    //second submain function
    let secondInputDataGroup low high = 
              
        //createList        
        let createListInDirWithIncorrNoOfFiles = 
    
            let folderItem = sprintf "%s%s%s-%s%s" //u sprintf je typova kontrola  
                                <| rcO.prefix
                                <| stringChoice (low |> function Low value -> value)
                                <| string (low |> function Low value -> value) 
                                <| stringChoice (high |> function High value -> value)
                                <| string (high |> function High value -> value)  
            
            let dirWithIncorrNoOfFiles = sprintf "%s%s" 
                                            <| string rcO.path
                                            <| folderItem     
                 
            try   //vyzkouseni si .NET exceptions
                try                       
                    (*
                    let dirInfo = new DirectoryInfo(dirWithIncorrNoOfFiles)                
                    let dirInfoOption = dirInfo 
                                        |> Option.ofObj   
                                        |> optionToDirInfo "DirectoryInfo()"
                        
                    let mySeq = dirInfoOption.EnumerateFiles("*.jpg") //nelze kvuli tomu, ze to dava jiny typ, nez potrebujeme
                                |> Option.ofObj   
                                |> optionToEnumerable "dirInfoOption.EnumerateFiles()"  
                        
                    match dirInfoOption.Exists with                         
                    | true  -> List.ofSeq(mySeq)                                                 
                    | false -> dirWithIncorrNoOfFiles |> error5   
                                List.Empty
                    *)
                                  
                    //2x staticka trida System.IO.Directory...., nebot nelze objekt dirInfo vyuzit 2x
                    let mySeq = Directory.EnumerateFiles(dirWithIncorrNoOfFiles, "*.jpg")
                                |> Option.ofObj   
                                |> optionToEnumerable "Directory.EnumerateFiles()"     
                        
                    match Directory.Exists(dirWithIncorrNoOfFiles) with   
                    | true  -> List.ofSeq(mySeq)                                                 
                    | false -> dirWithIncorrNoOfFiles |> error5   
                               List.Empty 
                finally
                () //zatim nepotrebne
            with  
            | :? System.IO.DirectoryNotFoundException as ex -> ex.Message |> error3
                                                               List.Empty                                                                                        
            | :? System.IO.IOException as                ex -> ex.Message |> error4 
                                                               List.Empty
            | _ as                                       ex -> ex.Message |> error1 //System.Exception
                                                               List.Empty           
                                
        createListInDirWithIncorrNoOfFiles
    
    let secondDataGroup = secondInputDataGroup
                          <| firstInputDataGroup.low 
                          <| firstInputDataGroup.high     
    
    firstInputDataGroup, secondDataGroup        
   
    (*       
       System.IO.File provides static members related to working with files, whereas System.IO.FileInfo represents a specific file and contains non-static members for working with that file.          
       Because all File methods are static, it might be more efficient to use a File method rather than a corresponding FileInfo instance method if you want to perform only one action. All File methods 
       require the path to the file that you are manipulating.    
       The static methods of the File class perform security checks on all methods. If you are going to reuse an object several times, consider using the corresponding 
       instance method of FileInfo instead, because the security check will not always be necessary.  
    *)
