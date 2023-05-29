using Logic.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace Logic.Services;

public class HashProvider : IHashProvider
{

    private readonly HashAlgorithm _hashAlgorithm;
    private readonly Encoding _encoding;

    public HashProvider(HashAlgorithm hashAlgorithm, Encoding encoding)
    {
        _encoding = encoding ?? throw new ArgumentNullException(nameof(encoding));
        _hashAlgorithm = hashAlgorithm ?? throw new ArgumentNullException(nameof(hashAlgorithm));
    }


    public string GetHash(string value)
    {
        var pwdBytes = _encoding.GetBytes(value ?? throw new ArgumentNullException(nameof(value)));
        var pwdHash = _hashAlgorithm.ComputeHash(pwdBytes);
        return _encoding.GetString(pwdHash);
    }
}
