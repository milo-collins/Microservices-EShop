using EShop.Infrastructure.Command.User;
using EShop.Infrastructure.Event.User;
using EShop.Infrastructure.Security;

namespace EShop.User.DataProvider
{
    public static class Extension
    {
        public static CreateUser SetPassword(this CreateUser user, IEncrypter encrypter)
        {
            var salt = encrypter.GetSalt();
            user.Password = encrypter.GetHash(user.Password, salt);

            return user;
        }

        public static bool ValidatePassword(this UserCreated userCreated, LoginUser user, IEncrypter encrypter)
        {
            //return savedUser.Password.Equals(encrypter.GetHash(user.Password, encrypter.GetSalt()));
            var pswd = encrypter.GetHash(user.Password, encrypter.GetSalt());
            var valid = userCreated.Password.Equals(pswd);
            return valid;
        }
    }
}
