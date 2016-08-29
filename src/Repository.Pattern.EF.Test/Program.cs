using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repository.Pattern.EF.Test
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //using (var context = new NortwindDbContext())
            //using (var unitOfWork = new UnitOfWork(context))
            //{
            //    IRepositoryAsync<Customer> customerRepository = new Repository<Customer>(context, unitOfWork);

            //    //var customer = new Customer
            //    //{
            //    //    UniqueId = Guid.NewGuid(),
            //    //    ParentId = 1,
            //    //    Name = "Test",
            //    //    Status = 1,
            //    //    CreateDate = DateTime.Now
            //    //};

            //    //customerRepository.Insert(customer);
            //    //unitOfWork.SaveChanges();

            //    //Transactional Insert
            //    //try
            //    //{
            //    //    unitOfWork.BeginTransaction();
            //    //    var customer = new Customer
            //    //    {
            //    //        UniqueId = Guid.NewGuid(),
            //    //        ParentId = 1,
            //    //        Name = "Test",
            //    //        Status = 1,
            //    //        CreateDate = DateTime.Now
            //    //    };

            //    //    customerRepository.Insert(customer);
            //    //    unitOfWork.SaveChanges();
            //    //    unitOfWork.Commit();
            //    //}
            //    //catch (Exception ex)
            //    //{
            //    //    unitOfWork.Rollback();
            //    //}


            //    //Get All Record
            //    var getAllRecord = customerRepository.GetAll().ToList();

            //    //Filter for paging record
            //    var query = customerRepository.Filter(null, null, null, 1, 10).ToList();




            //    //Fluent query
            //    IQueryFluent<Customer> queryFluent = new QueryFluent<Customer>(new Repository<Customer>(context, unitOfWork));
            //    int totalRecord = 0;
            //    //Fluent query paging record
            //    var selectPaging = queryFluent.SelectPage(1, 10, out totalRecord).ToList();


            //    IRepositoryAsync<UserRole> userRoleRepository = new Repository<UserRole>(context, unitOfWork);
            //}
        }
    }
}
