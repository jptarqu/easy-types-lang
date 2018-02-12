namespace DotNetParser

module SemanticTypes = 
    open PrimitivesParserTypes

    type CustomPrimitive = {
        name: string
        baseType:  CommonDataRequirements
    }

    let parseNumParam (str:string) =
        match str.Trim().ToLower() with 
        | "max" -> System.Int32.MaxValue
        | "min" -> System.Int32.MinValue
        | _ -> 
            let isNUm, num = System.Int32.TryParse(str)
            match isNUm with
            | true -> num
            | false -> 0
    let mapToSemantic (info: CustomPrimitiveInfo) : CustomPrimitive =
        match info.baseType with 
        | "String"      -> { name = info.name; baseType = CommonDataRequirementsString {Size = parseNumParam info.baseArgs.[0];   MinSize = parseNumParam info.baseArgs.[1];  } }
        | "Integer"     ->
        | "Decimal"     ->
        | "Date"        ->
        | "DateTime"    ->
        | "Binary"     ->