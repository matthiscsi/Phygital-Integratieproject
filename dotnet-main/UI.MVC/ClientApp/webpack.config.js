const path = require('path');
const MiniCssExtractPlugin = require("mini-css-extract-plugin");


module.exports = {
    resolve: {
        extensions: ['.ts', '.js'],
    },
    entry: {
        site: './src/js/site.js',
        conditionalpoint: './src/ts/entrypoints/conditionalpointentry.ts',
        passflowsession: './src/ts/entrypoints/passflowsessionentry.ts',
        passflowsessioncompanion: './src/ts/entrypoints/passflowsessioncompanionentry.ts',
        passflowsessionclient: './src/ts/entrypoints/passflowsessioncliententry.ts',
        passflowsessionhost: './src/ts/entrypoints/passflowsessionhostentry.ts',
        waitscreen: './src/ts/entrypoints/waitscreenentry.ts',
        addflow: './src/ts/entrypoints/addflowentry.ts',
        flowindex: './src/ts/entrypoints/flowindexentry.ts',
        flowdetails: './src/ts/entrypoints/flowdetailsentry.ts',
        thankyou: './src/ts/entrypoints/thankyouentry.ts',
        flowstep: './src/ts/entrypoints/flowstepentry.ts',
        editflowstep: './src/ts/entrypoints/editflowstepentry.ts',
        projectform: './src/ts/entrypoints/projectformentry.ts',
        rangequestionanalysis: './src/ts/entrypoints/rangequestionanalysisentry.ts',
        closedquestionanalysis: './src/ts/entrypoints/closedquestionanalysisentry.ts',
        flowanalysis: './src/ts/entrypoints/flowanalysisentry.ts',
        multiplechoicequestionanalysis: './src/ts/entrypoints/multiplechoicequestionanalysisentry.ts',
        hidelogin: './src/ts/hidelogin.ts',
        openquestionanalysis: './src/ts/entrypoints/openquestionanalysisentry.ts',
        projectanalysis: './src/ts/entrypoints/projectanalysisentry.ts',
        subplatformanalysis: './src/ts/entrypoints/subplatformanalysisentry.ts',
        userexperience: './src/ts/entrypoints/userexperience.ts'
    },
    output: {
        filename: '[name].entry.js',
        path: path.resolve(__dirname, '..', 'wwwroot', 'dist')
    },
    devtool: 'source-map',
    mode: 'development',
    module: {

        rules: [
            {
                test: /.tsx?$/i,
                use: 'ts-loader',
                exclude: /node_modules/
            },
            {
                test: /\.s?css$/i,
                use: [
                    MiniCssExtractPlugin.loader,
                    "css-loader",
                    "sass-loader"
                ]
            },
            {
                test: /\.(png|jpg|gif)$/i,
                use: [
                    {
                        loader: 'file-loader',
                        options: {
                            name: '[path][name].[ext]',
                            outputPath: 'images/',
                        }
                    }
                ]
            },
            {
                test: /\.(eot|woff(2)?|ttf|otf)$/i,
                type: 'asset/resource',
                generator: {
                    filename: 'fonts/[name].[hash][ext]',
                }
            }
        ]
    },
    plugins: [
        new MiniCssExtractPlugin({
            filename: '[name].css',
        })
    ]
};