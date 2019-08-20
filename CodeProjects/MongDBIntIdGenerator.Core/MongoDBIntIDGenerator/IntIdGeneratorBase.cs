using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace MongoDBIntIdGenerator
{
    /// <summary>
    /// Base class for id generator based on integer values.
    /// </summary>
    public abstract class IntIdGeneratorBase : IIdGenerator
    {
        #region Fields
        private string m_idCollectionName;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="MongoDBIntIdGenerator.IntIdGeneratorBase"/> class.
        /// </summary>
        /// <param name="idCollectionName">Identifier collection name.</param>
        protected IntIdGeneratorBase(string idCollectionName)
        {
            m_idCollectionName = idCollectionName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoDBIntIdGenerator.IntIdGeneratorBase"/> class.
        /// </summary>
        protected IntIdGeneratorBase() : this("IDs")
        {
        }
        #endregion

        #region Methods

        /// <summary>
        /// Creates the update builder.
        /// </summary>
        /// <returns>The update builder.</returns>
        protected abstract UpdateDefinition<dynamic> CreateUpdateBuilder();

        /// <summary>
        /// Converts to int.
        /// </summary>
        /// <returns>The to int.</returns>
        /// <param name="value">Value.</param>
        protected abstract object ConvertToInt(BsonValue value);

        /// <summary>
        /// Tests whether an Id is empty.
        /// </summary>
        /// <param name="id">The Id.</param>
        /// <returns>True if the Id is empty.</returns>
        public abstract bool IsEmpty(object id);

        /// <summary>
        /// Generates an Id for a document.
        /// 
        /// 调用dynamic来获取Collection
        /// https://stackoverflow.com/questions/54404419/mongodb-sequential-int-id-generator-for-c-sharp-class-hierarchy
        /// </summary>
        /// <param name="container">The container of the document (will be a MongoCollection when called from the C# driver).</param>
        /// <param name="document">The document.</param>
        /// <returns>An Id.</returns>
        public object GenerateId(object container, object document)
        {
            var containerDynamic = (dynamic)container;
            var idSequenceCollection = containerDynamic.Database.GetCollection<dynamic>(m_idCollectionName);

            //var genericCollType = typeof(IMongoCollection<>).MakeGenericType(document.GetType());
            //var mongodb = genericCollType.GetProperty("Database").GetValue(container) as IMongoDatabase;
            //var idSequenceCollection = mongodb.GetCollection<BsonDocument>(m_idCollectionName);

            var filter = Builders<dynamic>.Filter.Eq("_id", m_idCollectionName);

            var update = CreateUpdateBuilder();
            var returnVal = idSequenceCollection.FindOneAndUpdate(
                filter,
                update,
                new FindOneAndUpdateOptions<dynamic>
                {
                    IsUpsert = true,
                    // 返回数据新增后的值, 1, 2, 3， 如果为默认值Before，返回的值从0开始，0，1，2  
                    ReturnDocument = ReturnDocument.After
                }
            );

            return ConvertToInt(returnVal.seq);
        }
        #endregion
    }
}

