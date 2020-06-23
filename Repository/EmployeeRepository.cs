using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using Test.Models;

namespace Test.Repository
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private string _connectionString = @"Data Source=DESKTOP-UIRKCDR\SQLEXPRESS;Initial Catalog=MVC_DB;Integrated Security=True";
        // private string _connectionString = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;
        public IEnumerable<Employee> GetAllEmployess()
        {
            List<Employee> employees = new List<Employee>();
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetAllEmployees", con);
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();


                while (rdr.Read())
                {
                    Employee employee = new Employee();
                    employee.ID = Convert.ToInt32(rdr["Id"]);
                    employee.UserName = rdr["UserName"].ToString();
                    employee.Name = rdr["Name"] == DBNull.Value ? null : rdr["Name"].ToString();
                    employee.Gender = rdr["Gender"] == DBNull.Value ? null : rdr["Gender"].ToString();
                    employee.City = rdr["City"] == DBNull.Value ? null : rdr["City"].ToString();
                    employee.Salary = Convert.ToDecimal(rdr["Salary"]);
                    employee.DOB = rdr["DateOfBirth"] != DBNull.Value ? Convert.ToDateTime(rdr["DateOfBirth"]) : DateTime.MinValue;
                    employee.Email = rdr["Email"].ToString(); ;
                    employee.Mobile = rdr["Mobile"].ToString();
                    employee.Role = rdr["Role"].ToString(); ;
                    employee.ImgName = rdr["Img"].ToString(); ;
                    employees.Add(employee);
                }
            }
            return employees;
        }

        public bool AddEmmployee(Employee emp)
        {
            bool res = false;
            try
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    string qry = "Insert into  Employee(UserName,Pwd,Name, Gender, City, Salary, DateOfBirth,Email,Mobile,Role,Img) ";
                    qry += " Values(@UserName,@Pwd,@Name, @Gender, @City, @Salary, @DateOfBirth, @Email, @Mobile, @Role, @Img)";
                    SqlCommand cmd = new SqlCommand(qry, con);
                    cmd.Parameters.Add(new SqlParameter("Name", emp.Name));
                    cmd.Parameters.Add(new SqlParameter("Gender", emp.Gender));
                    cmd.Parameters.Add(new SqlParameter("City", emp.City));
                    cmd.Parameters.Add(new SqlParameter("Salary", emp.Salary));
                    cmd.Parameters.Add(new SqlParameter("DateOfBirth", emp.DOB));
                    cmd.Parameters.Add(new SqlParameter("Email", emp.Email));
                    cmd.Parameters.Add(new SqlParameter("Mobile", emp.Mobile));
                    cmd.Parameters.Add(new SqlParameter("UserName", emp.UserName));
                    cmd.Parameters.Add(new SqlParameter("Pwd", emp.Pwd));
                    cmd.Parameters.Add(new SqlParameter("Role", emp.Role));
                    cmd.Parameters.Add(new SqlParameter("Img", emp.ImgName));
                    //Open the connection and execute the command on ExecuteNonQuery method
                    con.Open();
                    cmd.ExecuteNonQuery();
                    res = true;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return res;

            }

            return res;


        }

        public bool UpdateEmmployee(Employee employee)
        {
            bool res = false;
            try
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    SqlCommand cmd = new SqlCommand("spUpdateEmployee", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlParameter paramId = new SqlParameter();
                    paramId.ParameterName = "@Id";
                    paramId.Value = employee.ID;
                    cmd.Parameters.Add(paramId);
                    SqlParameter paramName = new SqlParameter();
                    paramName.ParameterName = "@Name";
                    paramName.Value = employee.Name;
                    cmd.Parameters.Add(paramName);
                    SqlParameter paramGender = new SqlParameter();
                    paramGender.ParameterName = "@Gender";
                    paramGender.Value = employee.Gender;
                    cmd.Parameters.Add(paramGender);
                    SqlParameter paramCity = new SqlParameter();
                    paramCity.ParameterName = "@City";
                    paramCity.Value = employee.City;
                    cmd.Parameters.Add(paramCity);
                    SqlParameter paramSalary = new SqlParameter();
                    paramSalary.ParameterName = "@Salary";
                    paramSalary.Value = employee.Salary;
                    cmd.Parameters.Add(paramSalary);
                    SqlParameter paramDateOfBirth = new SqlParameter();
                    paramDateOfBirth.ParameterName = "@DateOfBirth";
                    paramDateOfBirth.Value = employee.DOB;
                    cmd.Parameters.Add(paramDateOfBirth);

                    SqlParameter paramRole = new SqlParameter();
                    paramRole.ParameterName = "@Role";
                    paramRole.Value = employee.Role;
                    cmd.Parameters.Add(paramRole);

                    SqlParameter paramImg = new SqlParameter();
                    paramImg.ParameterName = "@Img";
                    paramImg.Value = employee.ImgName;
                    cmd.Parameters.Add(paramImg);
                    con.Open();
                    cmd.ExecuteNonQuery();
                    res = true;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return res;

            }

            return res;
        }

        public bool DeleteEmployee(int id)
        {
            bool res = false;
            try
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    SqlCommand cmd = new SqlCommand("Delete from Employee where Id = @Id", con);
                    cmd.Parameters.Add(new SqlParameter("Id", id));
                    con.Open();
                    cmd.ExecuteNonQuery();
                    res = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return res;

            }
            return res;
        }

        public bool Login(Employee emp)
        {
            bool IsAuth = false;
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("select count(*) from Employee where UserName=@UserName and Pwd=@Pwd ", con);
                cmd.Parameters.Add(new SqlParameter("UserName", emp.UserName));
                cmd.Parameters.Add(new SqlParameter("Pwd", emp.Pwd));
                con.Open();
                int count = (int)cmd.ExecuteScalar();
                if (count > 0)
                    IsAuth = true;
                con.Close();
            }
            return IsAuth;
        }

    }

    public interface IEmployeeRepository
    {
        IEnumerable<Employee> GetAllEmployess();
        bool AddEmmployee(Employee employeeToCreate);
        bool UpdateEmmployee(Employee employeeToUpdate);
        bool DeleteEmployee(int id);
        bool Login(Employee emp);


    }

}