namespace DotNetParser

module SemanticTypes = 
    open PrimitivesParserTypes
    open CustomTypesParserTypes
    open DotNetParser

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
