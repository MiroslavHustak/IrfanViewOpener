namespace KillSingleProcess

open System
open System.Diagnostics

module Process =

    //quli DLL pro potencialni pouziti v C# nazev je velkym + tuple
    let KillSingleProcess(name: string, errorNumber: string, consoleApp: bool): unit = 
       try          
          let iterateThroughProcess =
              Process.GetProcessesByName(name) 
              |> Array.toList 
              |> List.map (fun item -> 
                                      match (item.ProcessName |> String.IsNullOrEmpty) with
                                      | true  -> ()
                                      | false -> item.Kill() 
                          )
          ()                                            
       with  
       |ex when (consoleApp = true)  -> 
                                        do printfn "%s: %s" <| errorNumber <| string ex.Message
                                        let result = Console.ReadKey()  
                                        ()
       |_  when (consoleApp = false) -> ()
