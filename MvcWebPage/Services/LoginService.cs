using MvcWebPage.Data;
using MvcWebPage.MLAVID;
using MvcWebPage.Models;

namespace MvcWebPage.Services
{
    public class LoginService
    {
        //public static Usuario? Login(Usuario usuario)
        //{
        //    return BD.ListaUsuarios().FirstOrDefault(s => s.User == usuario.User && s.Password == usuario.Password);
        //}

        public static void DefaultCuentas(string password)
        {
            try
            {
                MLAVIDContext db = new MLAVID_DB();
                var id = db.VENDEDORES.Max(m => m.CODVENDEDOR);
                db.VENDEDORES.Add(new VENDEDORES
                {
                    CODVENDEDOR         = id + 1,
                    NOMVENDEDOR         = "CUENTAS1",
                    PASSWORDENTRADARFID = password.GetMD5Hash(),
                    COLOR               = ".",
                    TALLA               = ".",
                    TIPOUSUARIO         = 1
                });


                db.SaveChanges();
            }
            catch (Exception e)
            {
                var err = e.Message;
            }
        }
    }
}
