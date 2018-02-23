namespace EasyTypes.FSharpGenerator

module PrimitiveGeneration =
    open DotNetParser.SemanticTypes
    open DotNetParser.PrimitivesParserTypes

    module CreateCodes = 
        open DotNetParser.PrimitivesParserTypes

        let ForStr (primitiveName: string) (req: CommonDataRequirementsString) =
            let baseType = "string"
            "
    type T = private " + primitiveName + " of " + baseType + "

    let Apply f (" + primitiveName + " s) =
        f s
    let Create (s: " + baseType + ") =
        let result = CommonValidations.isCorrectLenght " + req.MinSize.ToString() + " " + req.Size.ToString() + " s 
        match result with
        | Bad errs  -> fail errs 
        | Ok (goodObj, _) -> pass (" + primitiveName + " goodObj)

    let FromString = create
        
    let ToString (" + primitiveName + " s) : string =
        s

    let ToRendition (" + primitiveName + " s) : " + baseType + " =
        s
        "
        let ForStrPattern (primitiveName: string) (req: CommonDataRequirementsStringPattern) =
            let baseType = "string"
            "
    type T = private " + primitiveName + " of " + baseType + "

    let Apply f (" + primitiveName + " s) =
        f s
    let private regexPatter = new Regex(\"" + req.RegexPattern.ToString() + "\"
    let Create (s: " + baseType + ") =
        s
        |> CommonValidations.isCorrectLenght " + req.MinSize.ToString() + " " + req.Size.ToString() + "
        |> CommonValidations.isCorrectPattern regexPatter
        >=> " + primitiveName + "

    let FromString = create
        
    let ToString (" + primitiveName + " s) : string =
        s

    let ToRendition (" + primitiveName + " s) : " + baseType + " =
        s
        "
        let ForInt (primitiveName: string) (req: CommonDataRequirementsInt) =
            let baseType = "int"
            "
    type T = private " + primitiveName + " of " + baseType + "

    let Apply f (" + primitiveName + " s) =
        f s
    let Create (s:" + baseType + ") =
        s
        |> CommonValidations.isWithinRange " + req.MinValue.ToString() + " " + req.MaxValue.ToString() + "
        >=> " + primitiveName + "

    let FromString (str:string) =
        str
        |> CommonValidations.ToInt
        >>= Create

    let Apply f (" + primitiveName + " s) =
        f s
            
    let ToString (" + primitiveName + " s) : string =
        s.ToString()

    let ToRendition (" + primitiveName + " s) : " + baseType + " =
        s
        "
        let ForDecimal (primitiveName: string) (req: CommonDataRequirementsDecimal) =
            let baseType = "decimal"
            "
    type T = private " + primitiveName + " of " + baseType + "

    let Apply f (" + primitiveName + " s) =
        f s
    let Create (s:" + baseType + ") =
        s
        |> CommonValidations.isWithinRange " + req.MinValue.ToString() + " " + req.MaxValue.ToString() + "
        >=> " + primitiveName + "

    let FromString (str:string) =
        str
        |> CommonValidations.ToDecimal
        >>= Create

    let Apply f (" + primitiveName + " s) =
        f s
            
    let ToString (" + primitiveName + " s) : string =
        s.ToString()

    let ToRendition (" + primitiveName + " s) : " + baseType + " =
        s
        "
        
        let ForMoney (primitiveName: string) (req: CommonDataRequirementsMoney) =
            let baseType = "decimal"
            "
    type T = private " + primitiveName + " of " + baseType + "

    let Apply f (" + primitiveName + " s) =
        f s
    let Create (s:" + baseType + ") =
        s
        |> CommonValidations.isWithinRange " + req.MinValue.ToString() + " " + req.MaxValue.ToString() + "
        >=> " + primitiveName + "

    let FromString (str:string) =
        str
        |> CommonValidations.ToDecimal
        >>= Create

    let Apply f (" + primitiveName + " s) =
        f s
            
    let ToString (" + primitiveName + " s) : string =
        s.ToString()

    let ToRendition (" + primitiveName + " s) : " + baseType + " =
        s
    "
        let ForDate (primitiveName: string) (req: CommonDataRequirementsDate) =
            let baseType = "System.DateTime"
            "
    type T = private " + primitiveName + " of " + baseType + "

    let Apply f (" + primitiveName + " s) =
        f s
    let Create (s:" + baseType + ") =
        let minDate = new System.DateTime(" + req.MinValue.ToString("yyyy,MM,dd") + ")
        let maxDate = new System.DateTime(" + req.MaxValue.ToString("yyyy,MM,dd") + ")
        s
        |> CommonValidations.isWithinRange minDate maxDate 
        >=> " + primitiveName + "

    let FromString (str:string) =
        str
        |> CommonValidations.ToDate
        >>= Create

    let Apply f (" + primitiveName + " s) =
        f s
            
    let ToString (" + primitiveName + " s) : string =
        s.ToString(\"yyyy-MM-dd\")

    let ToRendition (" + primitiveName + " s) : " + baseType + " =
        s
        "
        
        let ForDateTimeOptional (primitiveName: string) (req: CommonDataRequirementsDateTime) =
            let baseType = "System.DateTime option"
            "
    type T = private " + primitiveName + " of " + baseType + " 

    let Apply f (" + primitiveName + " s) =
        f s
    let Create (s:" + baseType + ") =
        match s with
        | None -> pass " + primitiveName + " s
        | Some v ->
            let minDate = new System.DateTime(" + req.MinValue.ToString("yyyy,MM,dd, HH, mm, ss") + ")
            let maxDate = new System.DateTime(" + req.MaxValue.ToString("yyyy,MM,dd, HH, mm, ss") + ")
            v
            |> CommonValidations.isWithinRange minDate maxDate 
            >=> " + primitiveName + "
            
    let FromString (str:string) =
        if String.IsNullOrEmpty(str) then pass None else (str
            |> CommonValidations.ToDateTime >=> Some)
        >>= Create

    let Apply f (" + primitiveName + " s) =
        f s
            
    let ToString (" + primitiveName + " s) : string =
        s.ToString()

    let ToRendition (" + primitiveName + " s) : " + baseType + " =
        s
    "

        let ForDateTime (primitiveName: string) (req: CommonDataRequirementsDateTime) =
            if req.Optional then 
                ForDateTimeOptional primitiveName req
            else 
                let baseType = "System.DateTime"
                "
    type T = private " + primitiveName + " of " + baseType + "

    let Apply f (" + primitiveName + " s) =
        f s
    let Create (s:" + baseType + ") =
        let minDate = new System.DateTime(" + req.MinValue.ToString("yyyy,MM,dd, HH, mm, ss") + ")
        let maxDate = new System.DateTime(" + req.MaxValue.ToString("yyyy,MM,dd, HH, mm, ss") + ")
        s
        |> CommonValidations.isWithinRange minDate maxDate 
        >=> " + primitiveName + "

    let FromString (str:string) =
        str
        |> CommonValidations.ToDate
        >>= Create

    let Apply f (" + primitiveName + " s) =
        f s
            
    let ToString (" + primitiveName + " s) : string =
        s.ToString()

    let ToRendition (" + primitiveName + " s) : " + baseType + " =
        s
    "
        let ForBinary(primitiveName: string) (req: CommonDataRequirementsBinary) =
            let baseType = "byte[]"
            "
    type T = private " + primitiveName + " of " + baseType + "

    let Apply f (" + primitiveName + " s) =
        f s
    let Create (s:" + baseType + ") =
        s
        |> CommonValidations.isCorrectByteLenght " + req.MinSize.ToString() + " " + req.Size.ToString() + "
        >=> " + primitiveName + "

    let FromString (str:string) =
        str.ToCharArray()
        >>= Create

    let Apply f (" + primitiveName + " s) =
        f s
            
    let ToString (" + primitiveName + " s) : string =
        s.ToString()

    let ToRendition (" + primitiveName + " s) : " + baseType + " =
        s
        "
    let Generate (p: CustomPrimitive) =
        let primitiveName = p.name
        let baseType = p.baseType.GetRenditionTypeName()
        let createLogic =
            match p.baseType with 
            | CommonDataRequirementsString r -> CreateCodes.ForStr primitiveName r
            | CommonDataRequirementsStringPattern r -> CreateCodes.ForStrPattern primitiveName r
            | CommonDataRequirementsInt r -> CreateCodes.ForInt primitiveName r
            | CommonDataRequirementsDecimal r -> CreateCodes.ForDecimal primitiveName r
            | CommonDataRequirementsDate r -> CreateCodes.ForDate primitiveName r
            | CommonDataRequirementsDateTime r -> CreateCodes.ForDateTime primitiveName r
            | CommonDataRequirementsBinary r -> CreateCodes.ForBinary primitiveName r
            | CommonDataRequirementsMoney r -> CreateCodes.ForMoney primitiveName r
        "namespace FsCommons.Core

module " + primitiveName + " =
    open Chessie

    " + createLogic + "
"