namespace DotNetParser

module BuiltinPrimitives =
    open PrimitivesParserTypes
    open CustomTypesParserTypes
    open SemanticTypes
    open System

    let UnkownPrimitive: CustomPrimitive = { name = "Unkown"; baseType =  CommonDataRequirementsString {Size = 0;   MinSize = 0;  } } 
    //let ShortString: CustomPrimitive = { name = "ShortString"; baseType =  CommonDataRequirementsString {Size = 20;   MinSize = 1;  } } 
    //let LongString: CustomPrimitive = { name = "LongString"; baseType =  CommonDataRequirementsString {Size = 20;   MinSize = 1;  } } 
    //let DatePrimitive: CustomPrimitive = { name = "Date"; baseType = 
    //    CommonDataRequirementsDate { MinValue = DateTime.MinValue; MaxValue = DateTime.MaxValue  } } 
    //let PositiveNumber: CustomPrimitive = { name = "PositiveNumber"; baseType = 
    //    CommonDataRequirementsDate { MinValue = DateTime.MinValue; MaxValue = DateTime.MaxValue  } } 

