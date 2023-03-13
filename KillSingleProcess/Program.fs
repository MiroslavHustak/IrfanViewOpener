namespace KillSingleProcess

open System
open System.Diagnostics

module Process =    
   
    [<CompiledName "KillSingleProcess">] //pro pouziti v projektu ExcelKilerDll
    let killSingleProcess(name: string, errorNumber: string, consoleApp: bool): unit = 
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
       | ex when (consoleApp = true)  -> 
                                         do printfn "%s: %s" <| errorNumber <| string ex.Message
                                         Console.ReadKey() |> ignore  
                                         ()
       | _  when (consoleApp = false) -> ()
