'use strict';

module.exports = function(grunt) {
	require('load-grunt-tasks')(grunt);
	require('time-grunt')(grunt);

	grunt.initConfig({
		clean: {
			ts: {
				files: [{
					src:["Builders/*.d.ts","WebSockets/*.d.ts","Proto/*.d.ts","*.d.ts", "!Proto/Proto.d.ts"],
				}]
			}
		},
		ts: {
			build: {
				src:["Builders/*.ts","WebSockets/*.ts","Proto/*.ts","*.ts"],
				reference: "build/ThingModel.d.ts",
				out:'build/ThingModel.js',
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
					sourceMapName: 'build/ThinModel.min.js.map'
				},
				files: {
					'build/ThingModel.min.js' : ['build/ThingModel.js']
				}
			}
		}
	});

	grunt.registerTask('build', [
		'clean:ts',
		'ts:build',
		'uglify'
	]);

	grunt.registerTask('default', ['build']);
}