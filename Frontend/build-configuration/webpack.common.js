const commonPaths = require("./common-paths");
const HtmlWebpackPlugin = require("html-webpack-plugin");

const config = {
  output: {
    path: commonPaths.outputPath,
    publicPath: "/",
  },
  target: "web",
  resolve: {
    extensions: [".js", ".jsx", ".ts", ".tsx"],
  },
  plugins: [
    new HtmlWebpackPlugin({
      template: `src/index.html`,
      favicon: `src/favicon.ico`,
    }),
  ],
};

module.exports = config;
