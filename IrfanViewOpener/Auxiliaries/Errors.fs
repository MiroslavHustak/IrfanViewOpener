module Errors

open System

let errorTemplate1 str = 
    do Console.Clear()
    do Console.SetCursorPosition(0, 4)
    do printf "%s" str
    do Console.SetCursorPosition(0, 5)
    do printf "Opakovaně stiskni jakékoliv tlačítko pro ukončení programu... \n"
    do Console.ReadKey() |> ignore 
    do System.Environment.Exit(1)  
let errorTemplate2 str ex n = 
    do Console.Clear()
    do printf "%s" str
    do printf "Popis chyby: %s \n" <| string ex
    do printf "Stiskni jakékoliv tlačítko pro ukončení programu... (ERROR00%s) \n" n
    do Console.ReadKey() |> ignore 
    do System.Environment.Exit(1) 
let errorTemplate2a str ex n = 
    do Console.Clear()
    do printf "%s" str
    do printf "Popis chyby ERROR00%s: %s \n" n (string  ex)
let errorTemplate3 str ex n = 
    do Console.Clear()
    do printf "%s" str
    do printf "Popis chyby: %s \n" <| string ex
    do printf "Stiskni jakékoliv tlačítko pro pokračování... (ERROR00%s) \n" n
    do Console.ReadKey() |> ignore

let error0 ex = "Nastal problém s IrfanView nebo zobrazením skenovaného souboru \n" |> errorTemplate2 <| string ex <| string 0            
let error1 ex = "Oops, nastal problém, který by už by měl být podchycen někde jinde \n" |> errorTemplate2 <| string ex <| string 1
let error2 ex = String.Empty |> errorTemplate3 <| string ex <| string 2            
let error3 ex = "Buď je problém s adresáři v rozhazovacím adresáři anebo byl zadán chybný interval \n" |> errorTemplate2a <| string ex <| string 3
let error4 ex = "Problém se soubory ve vybraném intervalu adresářů \n" |> errorTemplate3 <| string ex <| string 4

let error5 s = 
    do printfn "ERROR005: Adresář %s nenalezen" s   
    do Console.ReadKey() |> ignore 

//a) variant with a "when" guard
let error6 s =
    let strContainsOnlySpace str = 
        str |> Seq.forall (fun item -> item = ' ')  //A string is a sequence of characters => use Seq.forall to test directly
    let str =               
        match s with
        | "" -> let str = "EMPTY STRING"
                str
        | _     when strContainsOnlySpace s -> let str = "SPACE"
                                               str
        | _  -> s                              
    do str |> printfn "ERROR006: Vložená hodnota [%s] buď není celé číslo, anebo přesahuje rozsah Int32. Zadej znovu." 

 //b) variant with match without active patterns (for learning purposes)
let error66 s =     
    let strContainsOnlySpace s = 
        s |> Seq.forall (fun item -> item = ' ')  //A string is a sequence of characters => use Seq.forall to test directly        
    let noActivePattern = 
        match strContainsOnlySpace s with
        | true  -> "SPACE"                      
        | false -> s    
    let s =               
        match s with
        | "" -> "EMPTY STRING"                
        | _  -> noActivePattern    
    do s |> printfn "ERROR066: Vložená hodnota [%s] buď není celé číslo, anebo přesahuje rozsah Int32. Zadej znovu." 

 //c) variant with active patterns (for learning purposes)
let error666 s =     
    let strContainsOnlySpace s = 
        s |> Seq.forall (fun item -> item = ' ')  //A string is a sequence of characters => use Seq.forall to test directly        
    let activePattern = 
        let (|Pattern1|Pattern2|) (value:string) =           
            match strContainsOnlySpace s with
            | true  -> Pattern1                      
            | false -> Pattern2   
        match s with
        | Pattern1 -> "SPACE"                      
        | Pattern2 -> s    
    let s =               
        match s with
        | "" -> "EMPTY STRING"                
        | _  -> activePattern    
    do s |> printfn "ERROR666: Vložená hodnota [%s] buď není celé číslo, anebo přesahuje rozsah Int32. Zadej znovu." 

let error7() = "ERROR007: Nastal problém při parsování dat ze souboru mustr.xlsx  \n" |> errorTemplate1 
let error8() = "ERROR008: Soubor mustr.xlsx nenalezen \n" |> errorTemplate1            
let error9() = "ERROR009: Nastal problém při adaptaci hodnot z xlsx v DataTable  \n" |> errorTemplate1 
let error10() = "ERROR010: Nastal problém při přenosu dat ze souboru mustr.xlsx \n" |> errorTemplate1 
let error11() = "ERROR011: Nastal problém paralelním běhu programu \n" |> errorTemplate1 

let error12 ex = 
    "ERROR012: Buď nedošlo k vypnutí Excelu, anebo došlo k pokusu o čtení xls(x) souboru, který je již používán pravděpodobně programem OpenOffice nebo jiným programem (teoreticky by to mohl být i Excel, pokud jej někdo spustil od doby jeho násilného ukončení) \n" 
    |> errorTemplate2 <| string ex <| string 12   

let error13() = "ERROR013: Někdo ti před nosem smazal příslušný jpg soubor, který chceš otevřít \n" |> errorTemplate1  
let error14() = "ERROR014: Hodnoty v DataTable jsou null \n" |> errorTemplate1 
let error15() = "ERROR015: Serializace se nezdařila \n" |> errorTemplate1 
let error16 str = (str |> sprintf "ERROR016: Chyba (%s) při downcasting \n") |> errorTemplate1 
let error17 str = (str |> sprintf "ERROR017: Nastal problém s %s \n") |> errorTemplate1 
