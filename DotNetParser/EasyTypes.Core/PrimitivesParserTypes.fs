namespace DotNetParser

module PrimitivesParserTypes =
    open Common
    
    type PrimitiveTypes =
        | String
        | Integer
        | Decimal
        | Money
        | Date
        | DateTime
        | Binary
        
    let PrimitiveTypesNames = [|
       "String"
       "Integer"
       "Decimal"
       "Money"
       "Date"
       "DateTime"
       "Binary"
       |]
    type CommonDataRequirementsString =
        {Size: int;   MinSize: int;  }

    type CommonDataRequirementsStringPattern =
        {Size: int;   MinSize: int; RegexPattern: System.Text.RegularExpressions.Regex; CharValidation: (char->bool)  }
    type CommonDataRequirementsInt =
        { MinValue: int; MaxValue: int;  }
    type CommonDataRequirementsDecimal =
        {Size: int; Precision: int;  MinValue: decimal; MaxValue: decimal;  }
    type CommonDataRequirementsMoney =
        { MinValue: decimal; MaxValue: decimal;  }
    type CommonDataRequirementsDate =
        {  MinValue: System.DateTime; MaxValue: System.DateTime; Optional: bool }
    type CommonDataRequirementsDateTime =
        {  MinValue: System.DateTime; MaxValue: System.DateTime; Optional: bool  }
    type CommonDataRequirementsBinary =
        {Size: int;   MinSize: int;  }

    type CommonDataRequirements =
        | CommonDataRequirementsString of CommonDataRequirementsString
        | CommonDataRequirementsStringPattern of CommonDataRequirementsStringPattern
        | CommonDataRequirementsInt of CommonDataRequirementsInt
        | CommonDataRequirementsDecimal of CommonDataRequirementsDecimal
        | CommonDataRequirementsDate of CommonDataRequirementsDate
        | CommonDataRequirementsDateTime of CommonDataRequirementsDateTime
        | CommonDataRequirementsBinary of CommonDataRequirementsBinary
        | CommonDataRequirementsMoney of CommonDataRequirementsMoney
        member x.GetRenditionTypeName() =
            match x with
            | CommonDataRequirementsString _ -> "string"
            | CommonDataRequirementsStringPattern _ -> "string"
            | CommonDataRequirementsInt _ -> "int"
            | CommonDataRequirementsDecimal _ -> "decimal"
            | CommonDataRequirementsDate dreq -> "System.DateTime " + if dreq.Optional then " option" else ""
            | CommonDataRequirementsDateTime dreq -> "System.DateTime " + if dreq.Optional then " option" else ""
            | CommonDataRequirementsBinary _ -> "byte[]"
            | CommonDataRequirementsMoney _ -> "decimal"

    type PrimitiveNameElement = {
        name: string
        position: PositionInfo
    }
    type BasePrimitiveTypeElement = {
        name: string
        position: PositionInfo
    }
    type BasePrimtiveArgElement = {
        value: string
        position: PositionInfo
    }

    type PrimitiveLine = {
        lineNum: int
        nameElement: PrimitiveNameElement
        primitiveBaseTypeElement: BasePrimitiveTypeElement option
        baseArguments: BasePrimtiveArgElement array
    }

    
    //enum BaseTypes = {
    //    text = 'text',
    //    integer = 'integer',
    //    decimal = 'decimal',
    //    date = 'date',
    //    dateAndTime = 'dateAndTime'
    //}
    type PrimitiveSyntaxLineType = 
        | PrimitiveLine of PrimitiveLine
    
    type CustomPrimitiveInfo = {
        name: string
        baseType: string
        baseArgs: string array
    }
    type ParsePrimtivesFileInfo = {
        linesParsed: PrimitiveSyntaxLineType array
        customPrimitives: CustomPrimitiveInfo seq
    }

