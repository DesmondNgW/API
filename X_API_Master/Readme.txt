��Ŀ��API�ӿ���Ŀ����http��ʽ����
��Ŀ��Ϊ���¼�����Σ�UI��Util��Interface��Business��Data��DataBase�����У�
Util���ǿ�ܹ��д��룬�ṩ������ε��á�
DataBase���Ƕ�д���ݿ�����ݿ�DAL���
Data���Ƕ��ṩ��wcf�ӿڽ������ݷ�װ�����а���Cache�㣬Core�Ǻ������ݣ�Extend�Ǹ�������
Business����Data�ṩ�����ݽ������ϴ�����װ��ҵ�����ݣ��������������棩
Interface����Business�������ϣ������ṩ��UI����API�Ľӿ�
UI�������ṩ�Ľӿڣ����յ�API��ڡ�����Ϊ����̨���ԡ�API�ӿڣ�API���ԣ�
ÿ�β�θ���ҵ����Ҫ�ԣ���ΪCore��Extend��Other��������Ҫ����Helper��
Ref�����ⲿdll��ŵ�ַ


/// <summary>
        /// ����Ԫ���ݷ�����ַ���ɴ�����
        /// </summary>
        /// <param name="address">Ԫ���ݵ�ַ</param>
        /// <param name="outPutFile">�������ļ�·��</param>
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