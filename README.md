
## Services Used

This backend relies on the following services:

- **Cloudinary**: Cloudinary is used for media asset storage. Follow documentation to get API key: [Cloudinary Documentation](https://cloudinary.com/documentation)

- **GetStreamIO**: GetStreamIO enables direct messaging features in your application. Explore their documentation to get API key: [GetStreamIO Documentation](https://getstream.io/docs/)

- **Firebase**: Firebase handles user authentication seamlessly. Follow the documentation to get API key: [Firebase Authentication Documentation](https://firebase.google.com/docs/auth)

- **Dale II**: Dale II assists in image generation. Get API key from here: [Dale II Documentation](https://platform.openai.com/docs/api-reference)

## Configuration

Before you can run this backend, you need to set up the necessary configuration values. In the `appsettings.json` file, update the following sections with your API keys and secrets:

```json
{
    "StreamPubKey": "your_stream_pub_key",
    "StreamPrivKey": "your_stream_priv_key",
    "FirebaseApiKey": "your_firebase_api_key",
    "CloudinaryCloud": "your_cloudinary_cloud_name",
    "CloudinaryApiKey": "your_cloudinary_api_key",
    "CloudinaryApiSecret": "your_cloudinary_api_secret",
    "DaleApiKey": "your_dale_api_key"
}
```
## Getting Started

To run this backend on your local machine or a server, follow these steps:

1. Clone this repository:

```bash
git clone https://github.com/YourUsername/SocialAppBackend.git
```
2. Run API in IDE of the choice

## Contributing

If you are interested in contributing to this project, please fork the repository and submit a pull request. We welcome contributions of all types, including bug fixes, feature requests, and documentation improvements.

