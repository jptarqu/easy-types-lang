namespace DotNetParser

module SemanticTypes = 
    open PrimitivesParserTypes
    open CustomTypesParserTypes
    open DotNetParser
    
    type CustomTextLookup = {
        name: string
        pairs:  IdLabelPair<string>  array
    }

    let LookupNotFound = {
        name = "NOT_FOUND"
        pairs = Array.empty
    }
    type CustomPrimitive = {
        name: string
        baseType:  CommonDataRequirements
    }
    
    type TypeProperty = {
        name: string
        propType: CustomPrimitive
    }
    type CustomType= {
        name: string;
        props: TypeProperty list
    }
