using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using teste_facial.Services;

namespace teste_facial
{
    public class SalvarDadosFoto
    {
        private readonly IDatabaseService _dbService;
        private string _conexao;

        public SalvarDadosFoto(IDatabaseService dbService)
        {
            _dbService = dbService;
            _conexao = _dbService.GetConnectionString();
        }

        public bool Salvar(string imagePath, string descriptor, string codigo)
        {
            using var db = new SqlConnection(_conexao);
            db.Open();
            var _sql = $@" insert into FaceDescriptors (ImagePath, Descriptor, codigo) values (@ImagePath, @Descriptor, @codigo) ";
            db.Execute(_sql, new { imagePath, descriptor, codigo });
            return true;
        }

        public Pessoa? Obter(string assinatura)
        {
            using var db = new SqlConnection(_conexao);
            db.Open();
            var _sql = $@" select ImagePath, Descriptor from FaceDescriptors where codigo = @assinatura";
            var retorno = db.QueryFirstOrDefault<Pessoa>(_sql, new { assinatura });
            return retorno;
        }

        public IEnumerable<Pessoa> ObterTodos()
        {
            using var db = new SqlConnection(_conexao);
            db.Open();
            var _sql = $@" select ImagePath, Descriptor from FaceDescriptors where codigo = @codigo";
            var retorno = db.Query<Pessoa>(_sql);
            return retorno;
        }

        public void ApagarFoto(string idAssinatua)
        {
            using var db = new SqlConnection(_conexao);
            db.Open();
            var _sql = $@" delete from FaceDescriptors where codigo = '{idAssinatua}' ";
            var retorno = db.Execute(_sql);
        }
    }
}
