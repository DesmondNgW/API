项目是API接口项目，以http方式请求。
项目分为以下几个层次：UI、Util、Interface、Business、Data、DataBase，其中：
Util：是框架共有代码，提供各个层次调用。
DataBase：是读写数据库的数据库DAL层次
Data：是对提供的wcf接口进行数据封装，其中包括Cache层，Core是核心数据，Extend是辅助数据
Business：对Data提供的数据进行整合处理，封装成业务数据（根据需求建立缓存）
Interface：对Business数据整合，对外提供（UI）的API的接口
UI：对外提供的接口，最终的API入口。（分为控制台测试、API接口，API测试）
每次层次根据业务重要性，分为Core、Extend、Other，根据需要建立Helper。
Ref：是外部dll存放地址


/// <summary>
        /// 根据元数据发布地址生成代理类
        /// </summary>
        /// <param name="address">元数据地址</param>
        /// <param name="outPutFile">代理类文件路径</param>
        static void GenerateCode(EndpointAddress address, string outPutFile)
         {
             MetadataExchangeClient mexClient = new MetadataExchangeClient(address);
             MetadataSet metadataSet = mexClient.GetMetadata();
             WsdlImporter importer = new WsdlImporter(metadataSet);
             CodeCompileUnit codeCompileUnit = new CodeCompileUnit();
             ServiceContractGenerator generator = new ServiceContractGenerator(codeCompileUnit);
            foreach (ContractDescription contract in importer.ImportAllContracts())
             {
                 generator.GenerateServiceContractType(contract);
             }            
             CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
            using (StreamWriter sw = new StreamWriter(outPutFile))
             {               
                using (IndentedTextWriter textWriter = new IndentedTextWriter(sw))
                 {
                     CodeGeneratorOptions options = new CodeGeneratorOptions();                   
                     provider.GenerateCodeFromCompileUnit(codeCompileUnit, textWriter, options);
                 }
             }
         }