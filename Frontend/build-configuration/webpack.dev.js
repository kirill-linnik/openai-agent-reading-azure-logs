const commonPaths = require("./common-paths");

const MiniCssExtractPlugin = require("mini-css-extract-plugin");
const ReactRefreshWebpackPlugin = require("@pmmmwh/react-refresh-webpack-plugin");
const { WebpackPluginServe: Serve } = require("webpack-plugin-serve");
const Dotenv = require("dotenv-webpack");

const port = process.env.PORT || 8080;

const config = {
  mode: "development",
  entry: {
    app: [`${commonPaths.appEntry}/index.tsx`, "webpack-plugin-serve/client"],
  },
  output: {
    filename: "[name].[fullhash].js",
  },
  devtool: "inline-source-map",
  module: {
    rules: [
      {
        test: /\.(js|jsx)$/,
        exclude: /node_modules/,
        use: ["babel-loader"],
      },
      {
        test: /\.(ts|tsx)$/,
        exclude: /node_modules/,
        use: ["ts-loader"],
      },
      {
        test: /\.(css|scss)$/,
        use: ["style-loader", "css-loader", "css-modules-typescript-loader"],
      },
      {
        test: /\.(jpg|jpeg|png|gif|mp3|svg)$/,
        use: ["file-loader"],
      },
    ],
  },
  plugins: [
    new MiniCssExtractPlugin({
      filename: "styles/[name].[fullhash].css",
      chunkFilename: "styles/[id].[fullhash].css",
    }),
    new ReactRefreshWebpackPlugin({
      overlay: { sockIntegration: "wps" },
    }),
    new Serve({
      historyFallback: true,
      liveReload: false,
      hmr: true,
      host: "localhost",
      port: port,
      open: true,
      static: commonPaths.outputPath,
    }),
    new Dotenv({
      path: "./build-configuration/dev.properties",
    }),
  ],
  watch: true,
};

module.exports = config;
