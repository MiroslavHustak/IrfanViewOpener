namespace FSharpIrfanViewOpener

open System

open Errors
open Helpers
open Settings
open Helpers.Process
open OpeningIrfanView
open ClosingFunctions
open ReadingDataFromExcel
open GettingInputValues
open ROP_Functions.MyFunctions

//C# 200, F# 800, celkem 1000 (pouze vlastni kod)
module Start =         

    [<EntryPoint>]
    let main argv =
    
        //******* 1) FIXING PROBLEM WITH ENCODING
        do System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance)
        
        Console.BackgroundColor <- ConsoleColor.Blue 
        Console.ForegroundColor <- ConsoleColor.White 
        Console.InputEncoding   <- System.Text.Encoding.Unicode
        Console.OutputEncoding  <- System.Text.Encoding.Unicode
      
        //******* 2) CLOSING EXCEL FILE AND IRFANVIEW FILE       
        let excelKiller x = 
            do ExcelKiller.ExcelKiller.SaveAndCloseExcel()  
                           
        let result = //tady je fn aji pro finally
            let ropResults = tryWith excelKiller (fun x -> do KillSingleProcess(MySettings.rc.imageViewerProcess, "ERROR006", true)) (fun ex -> ())                                                     
            ropResults |> deconstructor1 error12  
        result

        //******* 3a) PIPING INPUT VALUES FROM EXCEL INTO COMPOSED FUNCTION (UI INPUTS + IRFANVIEW OPENING)      
        //let howMyProgramRuns() = readDataFromExcel() |> getInputValues |> openIrfanView  //lze aji takto: readDataFromExcel() |> (getInputValues >> openIrfanView)  
        
        //******* 3b) COMPOSING FUNCTIONS   
        let howMyProgramRuns = readDataFromExcel >> getInputValues >> openIrfanView           
                                         
        do ExcelKiller.ExcelKiller.SimulateKeyStroke('\r') //quli whileKeyIsEnter() je treba pocitacove stiskout ENTER pri spusteni programu

        let rec whileKeyIsEnterOrEsc() =  
            let repeat() = 
                howMyProgramRuns() |> ignore 
                do printfn "\nStiskni ENTER nebo ESC pro další interval, jakoukoliv jinou klávesu pro ukončení"
                whileKeyIsEnterOrEsc()
            match Console.ReadKey().Key with   
            | ConsoleKey.Enter  -> repeat()                          
            | ConsoleKey.Escape -> repeat()                                  
            | _                 -> () //odtud program dojede k te nule na konci programu      
                                                          
        whileKeyIsEnterOrEsc()                                                           
        0//konec programu
      
        