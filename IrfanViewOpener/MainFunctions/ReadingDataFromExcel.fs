module ReadingDataFromExcel

open System.IO
open System.Data
open ExcelDataReader

open Errors
open Settings.MySettings
open ROP_Functions.MyFunctions

//nested functions, vystup jsou data (typu option)
//impure z logickych duvodu
let readDataFromExcel x =  
    
    let filepath = Path.GetFullPath(rcR.path1)   

    let readData() =    
    
        //tady pouzito System.IO.File.Open(), staticka trida, pouziti jednorazove
        use stream = File.Open(filepath, FileMode.Open, FileAccess.Read) //vyjimecne unmanaged scope, aby bylo mozne pouzit use    

        let excelReaderStream =  //vsimni si, jak si compiler vydedukuje string parametr z pouziti function (tj. match excelReaderStream fileExt with ...)
                                 //a vsimni si, jak takto vznikla partially applied function
            let excelReaderXlsxF stream = ExcelReaderFactory.CreateOpenXmlReader(stream)  
            let excelReaderXlsF stream = ExcelReaderFactory.CreateBinaryReader(stream)//pouze pro rozsireni programu o vyber excel souboru         
  
            function //vsimni si, jak si compiler vydedukuje parametr fileExt
            | ".xlsx" -> let myStream = stream |> excelReaderXlsxF  
                         myStream |> Option.ofObj
            | ".xls"  -> let myStream = stream |> excelReaderXlsF //tohle v teto app nikdy nenastane, neb mam nastaveny natvrdo mustr.xlsx, ne xls
                         myStream |> Option.ofObj
            | _       -> let myStream = None  
                         myStream 
    
        let dtXlsxOption: DataTable option  = 

            let fileExt = Path.GetExtension(filepath)

            match excelReaderStream fileExt with 
            | Some excelReader ->    
                                use dtXlsx = excelReader.AsDataSet(                
                                                new ExcelDataSetConfiguration (ConfigureDataTable = 
                                                    fun (_:IExcelDataReader) -> ExcelDataTableConfiguration (UseHeaderRow = true)
                                                )
                                                ).Tables.[rcR.indexOfXlsxSheet] 
                                //excelReader.Close()   
                                //excelReader.Dispose()                                          
                                dtXlsx |> Option.ofObj
            | None             -> 
                                do error10()
                                None 
        dtXlsxOption  
  
    let adaptDtXlsx (dtXlsxOption: DataTable option) =    

        let adapt = 
            match dtXlsxOption with 
            | Some dtXlsx ->                                              
                            let numberOfColumns = rc.columnIndex.Length
                            seq {0 .. numberOfColumns - 1} 
                            |> Seq.iter(fun item -> dtXlsx.Columns.[rc.columnIndex.[item]].SetOrdinal(item))//Usporadame sloupce
                                                                                       
                            let sequenceGenerator _ = dtXlsx.Columns.Count  
                            let condition = (<) numberOfColumns //partial application = numberOfColumns < sequenceGenerator, viz posledni radek Seq.initInfinite sequenceGenerator |> ....
                            let bodyOfWhileCycle _ = do dtXlsx.Columns.RemoveAt(numberOfColumns) //Vymazeme vsechny napravo, co tam nemaji co delat

                            Seq.initInfinite sequenceGenerator 
                            |> Seq.takeWhile condition 
                            |> Seq.iter bodyOfWhileCycle 

                            dtXlsx |> Option.ofObj
            | None       -> None                                 
        adapt 
        
    let readFromExcel =
        try
            try
                (*
                let fInfodat: FileInfo = new FileInfo(filepath)                 
                let fInfodatOption = fInfodat 
                                     |> Option.ofObj   
                                     |> optionToFileInfo "FileInfo()"
                *)   
                //match fInfodatOption.Exists with   
                match File.Exists(filepath) with 
                | true  -> let readFromExcel = readData() |> adaptDtXlsx  
                           readFromExcel                        
                | false -> do error8()
                           None
                                              
            finally
            () //zatim nepotrebne
        with                                                                                               
        | :? System.IO.IOException as ex -> ex.Message |> error12 
                                            None
        | _ as                        ex -> ex.Message |> error1 //System.Exception
                                            None
    readFromExcel            

    (*       
     System.IO.File provides static members related to working with files, whereas System.IO.FileInfo represents a specific file and contains non-static members for working with that file.          
     Because all File methods are static, it might be more efficient to use a File method rather than a corresponding FileInfo instance method if you want to perform only one action. All File methods 
     require the path to the file that you are manipulating.    
     The static methods of the File class perform security checks on all methods. If you are going to reuse an object several times, consider using the corresponding 
     instance method of FileInfo instead, because the security check will not always be necessary.    
     *) 