import {paraglideVitePlugin} from '@inlang/paraglide-js';
import {svelteTesting} from '@testing-library/svelte/vite';
import {sveltekit} from '@sveltejs/kit/vite';
import {defineConfig, loadEnv} from 'vite';

export default defineConfig(({mode}) => {
    const env = loadEnv(mode, process.cwd(), '');
    return {
        plugins: [
            sveltekit(),
            paraglideVitePlugin({
                project: './project.inlang',
                outdir: './src/lib/paraglide'
            })
        ],
        server: {
            port: parseInt(env.PORT),
        },
        test: {
            workspace: [
                {
                    extends: './vite.config.ts',
                    plugins: [svelteTesting()],
                    test: {
                        name: 'client',
                        environment: 'jsdom',
                        clearMocks: true,
                        include: ['src/**/*.svelte.{test,spec}.{js,ts}'],
                        exclude: ['src/lib/server/**'],
                        setupFiles: ['./vitest-setup-client.ts']
                    }
                },
                {
                    extends: './vite.config.ts',
                    test: {
                        name: 'server',
                        environment: 'node',
                        include: ['src/**/*.{test,spec}.{js,ts}'],
                        exclude: ['src/**/*.svelte.{test,spec}.{js,ts}']
                    }
                }
            ]
        }
    };
});
