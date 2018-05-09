namespace EasyTypes.FSharpGenerator

module OptionalStringPrimitiveGenerator =
    open DotNetParser.PrimitivesParserTypes
    

    let Generate (primitiveName: string) (req: CommonDataRequirementsString) =
            let baseType = "string option"
            "
    type T = private " + primitiveName + " of " + baseType + "

    let Apply f (" + primitiveName + " s) =
        f s
    let Create (s:" + baseType + ") =
        match s with
        | None -> pass (" + primitiveName + " s)
        | Some v ->
            v
            |> CommonValidations.isCorrectLenght " + req.MinSize.ToString() + " " + req.Size.ToString() + "
            >=> Some
            >=> " + primitiveName + "

            
    let FromString = Create
            
    let ToString (" + primitiveName + " s) : string =
        match s with
        | Some v -> v
        | None -> \"\"

    let ToRendition (" + primitiveName + " s) : " + baseType + " =
        s
        "