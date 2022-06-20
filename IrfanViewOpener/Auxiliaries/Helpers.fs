module Helpers

open System
open System.Diagnostics

open Errors

    //IntExtensions //REDUNDANT CODE (FOR LEARNING PURPOSES ONLY)
    module IntExtensions = 
        type System.Int32 with
             member this.IsEven = this % 2 = 0
             member this.IsOdd =  this % 2 <> 0
    
    module MyString = 
        //priklad pouziti: GetString(8, "0")//tuple a nazev velkym kvuli DLL pro C#
        let GetString (numberOfStrings: int, stringToAdd: string): string =   
            let initialString = String.Empty                //initial value of the string
            let listRange = [ 1 .. numberOfStrings ]
            let rec loop list auxString auxStringToAdd =
                match list with 
                | []        -> auxString
                | _ :: tail -> let finalString = (+) auxString auxStringToAdd
                               loop <| tail <| finalString <| auxStringToAdd
            loop <| listRange <| initialString <| stringToAdd
     
   module Process = 
        //nazev velkym plus tuple kvuli DLL pro C#
        //tento program pouziva nize uvedeny kod v separatni DLL F#
        let KillSingleProcess(name: string, errorNumber: string, consoleApp: bool): unit = 
           try          
              let iterateThroughProcess =
                  Process.GetProcessesByName(name)
                  //|> Option.ofObj        //zbytecne, nebot se dale ve funkci zachyti String.IsNullOrEmpty            
                  //|> optionToArrayToList
                  |> List.ofArray
                  |> List.map (fun item -> 
                                          match (item.ProcessName |> String.IsNullOrEmpty) with
                                          | true  -> ()
                                          | false -> item.Kill() 
                              )
              ()                                            
           with  
           | ex when (consoleApp = true)  -> do printfn "%s: %s" <| errorNumber <| string ex.Message
                                             let result = Console.ReadKey()  
                                             ()
           |_  when (consoleApp = false)  -> ()

        let internal pressEnterToContinue() = //internal indicates that the entity can be accessed only from the same assembly            
            Seq.initInfinite (fun _ -> Console.ReadKey().Key)  
            |> Seq.takeWhile ((<>) ConsoleKey.Enter) 
            |> Seq.iter (fun _  -> ())             

        let internal browseThroughScans() =   
            match Console.ReadKey().Key with
            | ConsoleKey.Escape -> do KillSingleProcess("i_view32", "ERROR007", true) 
            | ConsoleKey.Enter  -> () 
            | _                 -> pressEnterToContinue()
         
   module private TryParserInt =
        let tryParseWith (tryParseFunc: string -> bool * _) =
            tryParseFunc >> function
            | true, value -> Some value
            | false, _    -> None
        let parseInt = tryParseWith <| System.Int32.TryParse  
        let (|Int|_|) = parseInt        

   module Parsing =
        let f x = let isANumber = x                                          
                  isANumber        
        let rec parseMe =
            function            
            | TryParserInt.Int i -> f i 
            | notANumber         -> notANumber |> error66  
                                    f parseMe <| Console.ReadLine()
                                   
   module Parsing1 =
        let f x = let isANumber = x                                          
                  isANumber              
        let rec parseMe1 = 
            function            
            | TryParserInt.Int i -> f i
            | _                  -> do Console.Clear() |> ignore
                                    error7()  
                                    0 //whatever...                                            
   
   //Toto neni pouzivany kod, ale jen pattern pro tvorbu TryParserInt, TryParserDate atd. Neautorsky kod.
   module private TryParser =
        let tryParseWith (tryParseFunc: string -> bool * _) = 
            tryParseFunc >> function
                            | true, value -> Some value
                            | false, _    -> None

        let parseDate   = tryParseWith <| System.DateTime.TryParse
        let parseInt    = tryParseWith <| System.Int32.TryParse
        let parseSingle = tryParseWith <| System.Single.TryParse
        let parseDouble = tryParseWith <| System.Double.TryParse
        // etc.

        // active patterns for try-parsing strings
        let (|Date|_|)   = parseDate
        let (|Int|_|)    = parseInt
        let (|Single|_|) = parseSingle
        let (|Double|_|) = parseDouble

    