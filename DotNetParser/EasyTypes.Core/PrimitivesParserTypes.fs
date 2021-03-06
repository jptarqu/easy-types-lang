﻿namespace DotNetParser

module PrimitivesParserTypes =
    open Common
    
    type PrimitiveTypes =
        | String
        | StringPattern
        | Integer
        | Decimal
        | Money
        | Date
        | DateTime
        | Binary
        | StringChoices
        
    let PrimitiveTypesNames = [|
       "String"
       "StringPattern"
       "Integer"
       "Decimal"
       "Money"
       "Date"
       "DateTime"
       "Binary"
       "StringChoices"
       |]
    type CommonDataRequirementsString =
        {Size: int;   MinSize: int; Optional: bool }

    type IdLabelPair<'IdType> = 'IdType * string

    type CommonDataRequirementsStringChoices =
        { Choices: IdLabelPair<string> array ; Size: int }

    type CommonDataRequirementsStringPattern =
        {Size: int;   MinSize: int; RegexPattern: System.Text.RegularExpressions.Regex; } //CharValidation: (char->bool)  }
    type CommonDataRequirementsInt =
        { MinValue: int; MaxValue: int;  Optional: bool }
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
        | CommonDataRequirementsStringChoices of CommonDataRequirementsStringChoices
        member x.GetRenditionTypeName() =
            match x with
            | CommonDataRequirementsString dreq -> "string" + if dreq.Optional then " option" else ""
            | CommonDataRequirementsStringPattern dreq -> "string" 
            | CommonDataRequirementsInt dreq -> "int" + if dreq.Optional then " option" else ""
            | CommonDataRequirementsDecimal _ -> "decimal"
            | CommonDataRequirementsDate dreq -> "System.DateTime " + if dreq.Optional then " option" else ""
            | CommonDataRequirementsDateTime dreq -> "System.DateTime " + if dreq.Optional then " option" else ""
            | CommonDataRequirementsBinary _ -> "byte[]"
            | CommonDataRequirementsMoney _ -> "decimal"
            | CommonDataRequirementsStringChoices _ -> "string"

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

