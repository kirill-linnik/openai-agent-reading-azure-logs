const path = require("path");
const commonConfig = require("./build-configuration/webpack.common");
const argv = require("webpack-nano/argv");
const { merge } = require("webpack-merge");

module.exports = () => {
  const { env } = argv;

  const envConfig = require(`./build-configuration/webpack.${env}.js`);
  const mergedConfig = merge(commonConfig, envConfig);

  return mergedConfig;
};
