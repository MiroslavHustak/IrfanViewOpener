module ROP_Functions

open System
open System.IO

open Errors
open DiscriminatedUnions
 
    module MyFunctions =
        
        // let tryCatch f exnHandler x = jsem pro svoje pokusy upravil takto...
        let tryWith f1 f2 x =
            try
                try
                   f1 x |> Success
                finally
                   f2 x
            with
            | ex -> Failure ex.Message  
        
        // dalsi muj kod pro moje pokusy
        let deconstructor  =  //FOR TESTING PURPOSES ONLY (testing railway oriented programming features)
            function
            | Success x  -> let (y, z), c = x
                            (y, z), c                               
            | Failure ex -> ex |> error2
                            (-1, -1), Map.empty    
                            
        let deconstructor1 error =  //FOR TESTING PURPOSES ONLY (testing railway oriented programming features)
            function
            | Success x  -> ()                                                                         
            | Failure ex -> ex |> error
        
        let optionToString =            
            function 
            | Some value -> match value with                             
                            | "" -> "0"
                            | _  -> value               
            | None       -> do error14()                             
                            String.Empty
        
        let optionToString1 = 
            function 
            | Some value -> value 
            | None       -> do error15()
                            String.Empty        
        
        let optionToEnumerable str (x: Collections.Generic.IEnumerable<string> option) = 
            match x with 
            | Some value -> value
            | None       -> do error17 str  
                            Seq.empty

        (*
        let optionToEnumerable str (x: seq<'a> option) = 
            match x with 
            | Some value -> value
            | None       -> do error17 str  
                            Seq.empty        

        let optionToFileInfo str (x: FileInfo option) = 
            match x with 
            | Some value -> value
            | None       -> do error17 str 
                            new FileInfo(String.Empty) //whatever of FileInfo type
       *)