namespace DotNetParser

module SemanticTypes = 
    open PrimitivesParserTypes
    open CustomTypesParserTypes
    open DotNetParser

    type CustomPrimitive = {
        name: string
        baseType:  CommonDataRequirements
    }
    
    type CustomType= {
        name: string;
        props: CustomPrimitive list
    }
