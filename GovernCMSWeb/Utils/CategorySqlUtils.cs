using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using GovernCMS.Models;
using log4net;

namespace GovernCMS.Utils
{
    public class CategorySqlUtils
    {
        private static ILog logger = LogManager.GetLogger(typeof(CategorySqlUtils));

        public IList<Category> FindCategories(int websiteId)
        {
            IDictionary<int, Category> categoryDictionary = new Dictionary<int, Category>();
            IList<Category> categories = new List<Category>();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ContosoAdsDataSource"].ConnectionString))
            {
                conn.Open();
                try
                {
                    using (SqlCommand command = new SqlCommand("SELECT * FROM Category where websiteId = @websiteId", conn))
                    {
                        command.Parameters.Add("@websiteId", SqlDbType.Int);
                        command.Parameters["@websiteId"].Value = websiteId;
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Category category = new Category();
                                category.CategoryId = (int) reader["CategoryId"];
                                category.CategoryName = (string) reader["CategoryName"];
                                if (DBNull.Value != reader["ParentCategoryId"])
                                {
                                    category.ParentCategoryId = (int?) reader["ParentCategoryId"];
                                }
                                category.Number = (int) reader["Number"];
                                category.CreateDate = (DateTime) reader["CreateDate"];
                                
                                categoryDictionary.Add(category.CategoryId, category);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.Error("Unable to find categories for website: " + websiteId, ex);
                }
            }

            // Build out Categories - structured
            foreach (Category category in categoryDictionary.Values)
            {
                if (category.ParentCategoryId.HasValue)
                {
                    Category parentCategory = categoryDictionary[category.ParentCategoryId.Value];
                    if (parentCategory.SubCategories == null)
                    {
                        parentCategory.SubCategories = new List<Category>();
                    }
                    parentCategory.SubCategories.Add(category);
                }
            }

            // Now, all Subcategories are nested under parents
            // Get the Top-level Parents and add them to the List for return
            foreach (Category category in categoryDictionary.Values)
            {
                if (category.ParentCategoryId == null)
                {
                    categories.Add(category);
                }
            }
            
            return categories;
        }
    }
}