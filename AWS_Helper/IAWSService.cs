using Amazon;

using Amazon.Runtime;

namespace AWS_Helper {

    public interface IAWSService {

        string PublicKey { get; }
        string SecretKey { get; }
        RegionEndpoint Region { get; }
        BasicAWSCredentials Credentials { get; }

    }
}