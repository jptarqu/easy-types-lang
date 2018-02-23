namespace EasyTypes.FSharpGenerator

module PrimitiveGeneration =
    open DotNetParser.SemanticTypes
    open DotNetParser.PrimitivesParserTypes

    module CreateCodes = 
        open DotNetParser.PrimitivesParserTypes

        let ForStr (primitiveName: string) (req: CommonDataRequirementsString) =
            let baseType = "string"
            "
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
        "
        let ForDateTime (primitiveName: string) (req: CommonDataRequirementsDateTime) =
            let baseType = "System.DateTime"
            "
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
    "
        let ForBinary(primitiveName: string) (req: CommonDataRequirementsBinary) =
            let baseType = "byte[]"
            "
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

    type T = private " + primitiveName + " of " + baseType + "

    let Apply f (" + primitiveName + " s) =
        f s
    " + createLogic + "
"