module Settings

open System
    
[<Struct>]  //vhodne pro 16 bytes => 4096 characters
type Common_Settings = 
    {        
        columnIndex: int[] //both for G and R
        imageViewerProcess: string //both for O and S
    }
    static member Default = 
        {
            columnIndex = [| 0..15 |] //Sloupce A,B,C,...P =  0,1,2, ... 15
            imageViewerProcess = "i_view32"
        }

[<Struct>]
type ReadingDataFromExcel_Settings = 
    {
        path1: string  
        indexOfXlsxSheet: int  
    }
    static member Default = 
        {
            path1 = $@"c:\Users\Martina\Kontroly skenu Litomerice\mustr.xlsx"                   
            indexOfXlsxSheet = 0<1> //Sheet v poradi 0 jako prvni, 1 = druhy, atd.
        } 

[<Struct>]
type OpenIrfanView_Settings = 
    {        
        path: string      //viz modul DU     
        path2: string     //viz modul DU         
        numberOfScannedFileDigits: int
        prefix: string    
        stringZero: string  
        suffixAndExtLength: int  
    }
    static member Default = 
        {
            path =  $@"c:\Users\Martina\Kontroly skenu Litomerice\rozhazovani\"
            path2 = $@"c:\Users\Martina\Kontroly skenu Litomerice\IrfanView\i_view32.exe"
            numberOfScannedFileDigits = 5<1>
            prefix = "LT-"
            stringZero = "0"
            suffixAndExtLength = 10<1> //delka retezce _00001.jpg
        }  

    module MySettings = 
     
     let rc = Common_Settings.Default

     let rcR = 
        {
            ReadingDataFromExcel_Settings.Default with 
               path1 = $@"e:\E\Mirek po osme hodine a o vikendech\Kontroly skenu\mustr.xlsx"  //pouze pro testovani u sebe na pocitaci                              
        }

     let rcO =
        { 
            OpenIrfanView_Settings.Default with 
                path =  $@"e:\E\Mirek po osme hodine a o vikendech\Kontroly skenu\rozhazovani\" //pouze pro testovani u sebe na pocitaci
                path2 = $@"e:\E\Mirek po osme hodine a o vikendech\Kontroly skenu\IrfanView\i_view32.exe" //pouze pro testovani u sebe na pocitaci
        }
     
     //ARCHIV LITOMERICE
    
     //let rc =  Common_Settings.Default 
     //let rcR = ReadingDataFromExcel_Settings.Default
     //let rcO = OpenIrfanView_Settings.Default               
     

