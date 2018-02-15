namespace DotNetParser

module SemanticBuilders =
    open PrimitivesParserTypes
    open CustomTypesParserTypes
    open SemanticTypes

    let parseNumParam (str:string) =
        match str.Trim().ToLower() with 
        | "max" -> System.Int32.MaxValue
        | "min" -> System.Int32.MinValue
        | _ -> 
            let isNUm, num = System.Int32.TryParse(str)
            match isNUm with
            | true -> num
            | false -> 0

    let parseDecParam (str:string) =
        match str.Trim().ToLower() with 
        | "max" -> System.Decimal.MaxValue
        | "min" -> System.Decimal.MinValue
        | _ -> 
            let isNUm, num = System.Decimal.TryParse(str)
            match isNUm with
            | true -> num
            | false -> 0M

    let parseDateParam (str:string) =
        match str.Trim().ToLower() with 
        | "max" -> System.DateTime.MaxValue.Date
        | "min" -> System.DateTime.MinValue.Date
        | _ -> 
            let isNUm, num = System.DateTime.TryParse(str)
            match isNUm with
            | true -> num.Date
            | false -> System.DateTime(1970,01, 01)
    let parseDateTimeParam (str:string) =
        match str.Trim().ToLower() with 
        | "max" -> System.DateTime.MaxValue
        | "min" -> System.DateTime.MinValue
        | _ -> 
            let isNUm, num = System.DateTime.TryParse(str)
            match isNUm with
            | true -> num
            | false -> System.DateTime(1970,01, 01)

    let mapPrimitiveToSemantic (info: CustomPrimitiveInfo) : CustomPrimitive =
        match info.baseType with 
        | "String"      -> 
            { name = info.name; baseType = CommonDataRequirementsString {Size = parseNumParam info.baseArgs.[0];   MinSize = parseNumParam info.baseArgs.[1];  } }
        | "Integer"     ->
            { name = info.name; baseType = CommonDataRequirementsInt { MinValue = parseNumParam info.baseArgs.[0]; MaxValue = parseNumParam info.baseArgs.[1];  } }
        | "Decimal"     ->
            let maxVal = parseDecParam info.baseArgs.[1]
            let maxValStr = maxVal.ToString()
            let size = maxValStr.Replace(".","").Length
            let precision = maxValStr.Length - maxValStr.LastIndexOf('.') + 1 
            { name = info.name; baseType = CommonDataRequirementsDecimal {MinValue = parseDecParam info.baseArgs.[0]; MaxValue = maxVal; Size = size; Precision = precision  } }
        | "Money"     ->
            let maxVal = parseDecParam info.baseArgs.[1]
            let maxValStr = maxVal.ToString()
            { name = info.name; baseType = CommonDataRequirementsMoney {MinValue = parseDecParam info.baseArgs.[0]; MaxValue = maxVal; } }
        | "Date"        ->
            { name = info.name; baseType = CommonDataRequirementsDate {MinValue = parseDateParam info.baseArgs.[0]; MaxValue = parseDateParam info.baseArgs.[1]  } }
        | "DateTime"    ->
            { name = info.name; baseType = CommonDataRequirementsDateTime {MinValue = parseDateTimeParam info.baseArgs.[0]; MaxValue = parseDateTimeParam info.baseArgs.[1]  } }
        | "Binary"     ->      
            { name = info.name; baseType = CommonDataRequirementsBinary {Size = parseNumParam info.baseArgs.[0];   MinSize = parseNumParam info.baseArgs.[1];  } }
        | _ ->
            failwithf "Invalid Type %s" info.baseType
    
    let mapTypeToSemantic (allPrimitveTypes: CustomPrimitive seq)  (info: CustomTypeInfo) : CustomType =
        {
            name = info.name;
            props = info.props 
                |> Seq.map 
                    (fun p -> 
                        allPrimitveTypes 
                        |> Seq.tryFind (fun cp -> cp.name.ToUpper() = p.propTypeName.ToUpper()) 
                        |> Option.defaultValue BuiltinPrimitives.UnkownPrimitive
                        ) 
                |> Seq.toList
        }
