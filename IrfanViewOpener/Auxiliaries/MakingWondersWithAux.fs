module MakingWondersWithAux //MOSTLY REDUNDANT CODE (FOR LEARNING PURPOSES ONLY)

open FSharp.Quotations
open FSharp.Quotations.Evaluator.QuotationEvaluationExtensions

let getAux low high =  
            
    //let aux = high - low + 1//aneb cemu to robit jednoduse, kdyz to mozeme slozite:
            
    //a) ...strcit pres potrubi //continuation passing style
    let auxA = 1 |> fun x -> high |> fun y -> y - low |> fun z -> z + x //1 se protlaci do x a dale, atd.
            
    //b) ...pouzit composition
    //...takto:
    let z = //partial application
        let aux1 x = 1 + x
        let aux2 x = x - low  
        aux1 >> aux2
    let auxB1 = z high            
    //...anebo takto (lze i Array.reduce):                                          
    let zB =
        let listOfFunctions =
            [
                fun x -> 1 + x
                fun x -> x - low                                               
            ]                                               
        List.reduce (>>) listOfFunctions//vsechny funkce z kolekce necha vykonavat v jedne //normalne musi to asi byt v try-with//viz kap. composing functions together
    let auxB2 = zB high
           
    //c)... pouzit point free syntax bud takto:            
    let auxC1 = (-) 0 >> (+) 1 <| -(high - low)//= 0 - argument = vysledek |> 1 + vysledek
    //...anebo takto
    let auxC2 = (+) 0 >> (+) 1 <| high - low
           
    //d)... ci pouzivat monoid :-)
    let auxD1 = [high; -low; 1] |> List.reduce (+) //je to sum, ale melo to byt v try-with bloku            
    let auxD2 = [high; -low; 1] |> List.fold (+) 0 //je to sum (stejne, jako vyse, ale bez nutnosty try-with), musi to ale mit initial value            
    
    //e)... anebo jeste lepe quotations :-))    
    let expr = Expr.Value(1) // (this creates arbitrary untyped quotation)     
    let expr2 = <@ (fun x y -> x - y + %%expr) @>  // (%% splicing untyped part)   
    let auxE1 = expr2.Compile() <| high <| low // compile the quotation & run returned function + plus rovnou jsem dodal parametry
    auxE1 //return 