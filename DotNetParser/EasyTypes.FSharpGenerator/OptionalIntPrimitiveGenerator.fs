namespace EasyTypes.FSharpGenerator

module OptionalIntPrimitiveGenerator =
    open DotNetParser.PrimitivesParserTypes
    

    let Generate (primitiveName: string) (req: CommonDataRequirementsInt) =
            let baseType = "int"
            "
    type T = private " + primitiveName + " of " + baseType + "

    let Apply f (" + primitiveName + " s) =
        f s
    let Create (s:" + baseType + ") =
        match s with
        | None -> pass (" + primitiveName + " s)
        | Some v ->
            v
            |> CommonValidations.isWithinRange " + req.MinValue.ToString() + " " + req.MaxValue.ToString() + "
            >=> Some
            >=> " + primitiveName + "

            
    let FromString (str:string) =
        if String.IsNullOrEmpty(str) then pass None else (str
            |> CommonValidations.ToInt >=> Some)
        >>= Create
        
            
    let ToString (" + primitiveName + " s) : string =
        match s with
        | Some v -> v.ToString()
        | None -> \"\"

    let ToRendition (" + primitiveName + " s) : " + baseType + " =
        s
        "