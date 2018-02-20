namespace EasyTypes.SqlGenerator

module SqlStoredProcGenerator =
    open DotNetParser.PrimitivesParserTypes
    open DotNetParser.SemanticTypes
    open SqlCommon

    
    let private isAutoDateColumn (p:TypeProperty) =
        p.name = "CreatedOn" || p.name = "ModifiedOn" 
        
    let private autoGenColumn (p:TypeProperty) =
        isAutoDateColumn p || isPrimaryKey p

    let private buildParam (p:TypeProperty): string =
        let colCode = "@" + p.name + " " + convertToSqlType(p.propType.baseType) 
        colCode
    
    let nameForSpAdd  (customType: CustomType ) : string =
        "spInsert" + (customType.name)
    let buildInsertSp (customType: CustomType ) : string =
        let parameters = customType.props |> Seq.filter (autoGenColumn >> not) |> Seq.map buildParam 
        let parameterValues = 
            customType.props 
            |> Seq.map (fun p -> 
                if isAutoDateColumn(p) then
                    "GetDate()"
                else
                    "@" + p.name
                )
        let header = "CREATE PROCEDURE  [" + nameForSpAdd(customType) + "]"
        "\n" + header + "\n" + (String.concat ",\n" parameters) + "\nAS\n" +
        "	
        insert into [" + customType.name + "]
	    select " + (String.concat ",\n" parameterValues) + "

	    select cast(@@IDENTITY as int) [IdGenerated]"