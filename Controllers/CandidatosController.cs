using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Data;
using Newtonsoft.Json.Linq;
using Dapr.Client;
using FinalApiIS.Models;
using Dapr;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FinalApiIS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CandidatosController : ControllerBase
    {
        private IConfiguration Configuration;
        private readonly string _connectionString;
        private readonly DaprClient _daprClient;
        public CandidatosController(IConfiguration _configuration, DaprClient daprClient)
        {
            _daprClient = daprClient;
            Configuration = _configuration;
            _connectionString = _configuration.GetConnectionString("mysqlConnection");
        }

        
        [HttpPost]
        [Produces("application/json")]
        [Route("insert_candidato")]
        [Topic("orderpubsub", "orders")]
        public async Task<IActionResult> InsertarCandidato(JObject request)
        {
            try
            {

                Console.WriteLine("llego el dato dpr");

                using (MySqlConnection conn = new MySqlConnection(_connectionString))
                {
                    using (MySqlCommand cmd = new MySqlCommand("sp_candidatos", conn))
                    {
                        //int.Parse(request.GetValue("partido").ToString())
                        //request.GetValue("nombre").ToString()
                        //int.Parse(request.GetValue("edad").ToString())
                        dynamic data;
                        cmd.CommandType = CommandType.StoredProcedure;
                        //conn.Open();
                        cmd.Parameters.AddWithValue("@id_in", null);
                        cmd.Parameters.AddWithValue("@id_partido_in",1 );
                        cmd.Parameters.AddWithValue("@nombre_in", "Digo jaja");
                        cmd.Parameters.AddWithValue("@edad_in", 80 );
                        cmd.Parameters.AddWithValue("@option_control", 1);


                        conn.Open();
                        DataSet setter = new DataSet();
                        MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                        adapter.Fill(setter, "documento");

                        if (setter.Tables["documento"].Rows.Count > 0 || setter != null)
                        {
                            data = new JObject();
                            data.response = 4;
                            data.message = "Registro Guardado Exitosamente";
                            return Ok(data);
                        }
                        else
                        {
                            data = new JObject();
                            data.value = 0;
                            data.response = 4;
                            data.message = setter.Tables[0].Rows[0][0];
                            return BadRequest(data);
                        }



                    }
                }
            }
            catch (Exception ex)
            {
                dynamic data = new JObject();
                data.value = ex.ToString();
                data.response = 6;
                data.message = "Proceso no realizado, excepcion";
                return BadRequest(data);
            }

        }





        // GET: api/<CandidatosController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<CandidatosController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<CandidatosController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<CandidatosController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<CandidatosController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
