MongoDBIntIdGenerator
=====================

园项目地址：https://github.com/alexjamesbrown/MongDBIntIdGenerator
原项目引用mongodb 驱动较旧(mongocsharpdriver)， 本次修改引用了新的Mongodb c#驱动(MongoDB.Driver.Core, MongoDB.Bson)

MongoDB Sequential integer Id Generator - Uses findAndModify to create sequential id's

Designed to provide sequential int IDs for documents, using the method outlined here: <http://www.alexjamesbrown.com/blog/development/mongodb-incremental-ids/>

Some unit tests are included, however this is not yet tested on scale, with replica sets etc...

## Links
    1. MongoDB.NET driver: <http://mongodb.github.io/mongo-csharp-driver/?jmp=docs>
     (MongoDB.Driver.Core, MongoDB.Bson)

    2. Quick tour: <http://mongodb.github.io/mongo-csharp-driver/2.8/getting_started/quick_tour/>

    3. Driver Reference: http://mongodb.github.io/mongo-csharp-driver/2.9/

    4. https://github.com/mongodb/mongo-csharp-driver

## Notes
    BsonId 及IdGenertor文档: <http://mongodb.github.io/mongo-csharp-driver/2.9/reference/bson/mapping/>

    类默认Id, id, _id属性为mongodb id

    By convention, a public member called Id, id, or _id will be used as the identifier. You can be specific about this using the BsonIdAttribute.


    public class MyClass 
    {
        [BsonId]
        public string SomeProperty { get; set; }
    }

### Id Generators(当id属性为空时才调用IIdGenerator.GenerateId方法)

    IIdGenerator 方法IsEmpty, ConvertToInt, 当IsEmpty返回true时, 才调用GenerateId给id赋值

    Int32IdGenerator 可以在IsEmpty中修改为<=0, 当传入负值时也会创建id
    如果接口默认定义传-1表示新建, 如果不增加<=0判断, 会直接创建id=-1的数据, 当插入第二条id=-1数据是会报duplicate key error collection错误

    When you Insert a document, the driver checks to see if the Id member has been assigned a value and, if not, generates a new unique value for it. Since the Id member can be of any type, the driver requires the help of an IIdGenerator to check whether the Id has a value assigned to it and to generate a new value if necessary. The driver has the following Id generators built-in:

    * ObjectIdGenerator
    * StringObjectIdGenerator
    * GuidGenerator
    * CombGuidGenerator
    * NullIdChecker(GenerateId方法直接throw, 当id为null时会报错，达到检测目的)
    * ZeroIdChecker<T>
    * BsonObjectIdGenerator

## Usage

    // 默认会创建表"IdInt32", 结构为{_id: "IdInt32", seq: 1}的数据
    // 每次seq插入数据时增加1, 当前seq的值为最近插入数据的Id
    BsonClassMap.RegisterClassMap<MyClass>(cm => {
        cm.AutoMap();
        cm.IdMemberMap.SetIdGenerator(new Int32IdGenerator());
    });

	BsonClassMap.RegisterClassMap<MyClass>(cm => {
        cm.AutoMap();
        cm.IdMemberMap.SetIdGenerator(new Int64IdGenerator());
    });

    // 如果ClassA 继承 BaseClass, BaseClass有Id字段，注册时需要用BaseClass, 而不是ClassA 否则会报错
    BsonClassMap.RegisterClassMap<BaseClass>(cm => {
        cm.AutoMap();
        cm.IdMemberMap.SetIdGenerator(new Int32IdGenerator());
    });
