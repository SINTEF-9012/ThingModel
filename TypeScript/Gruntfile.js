'use strict';

module.exports = function(grunt) {
	require('load-grunt-tasks')(grunt);
	require('time-grunt')(grunt);

	grunt.initConfig({
		clean: {
			ts: {
				files: [{
					src:["Builders/*.{d.ts,js.map}","WebSockets/*.{d.ts,js.map}","Proto/*.{d.ts,js.map}","*.{d.ts,js.map}", "!Proto/Proto.d.ts"],
				}]
			}
		},
		ts: {
			build: {
				src:["Builders/*.ts","WebSockets/*.ts","Proto/*.ts","*.ts"],
				reference: "build/ThingModel.d.ts",
				out:'./build/ThingModel.js',
				// outDir:'build',
				options:{
					target: 'es5',
					module: 'commonjs',
					sourceMap:true
				}
			}
		},
		uglify: {
			ts: {
				options: {
					sourceMap: true,
					sourceMapName: 'build/ThingModel.min.js.map'
				},
				files: {
					'build/ThingModel.min.js' : ['build/ThingModel.js']
				}
			}
		},
		file_append: {
			nodify: {
				files: {
					'build/ThingModel.node.js': {
						input: './build/ThingModel.js',
						prepend: "var WebSocket = require('ws'),_ = require('lodash');\n"+
								"console.debug = console.log;\n"+
								"var dcodeIO = {ProtoBuf: require('protobufjs')};\n\n",
						append: 'module.exports = ThingModel;\n'
					}
				}
			}
		}
	});

	grunt.registerTask('build', [
		'clean:ts',
		'ts:build',
		'uglify',
		'file_append:nodify'
	]);

	grunt.registerTask('default', ['build']);
}