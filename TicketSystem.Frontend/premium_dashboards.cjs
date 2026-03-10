const fs = require('fs');
const path = require('path');

const srcDir = path.join(__dirname, 'src');

function walkDir(dir, callback) {
    fs.readdirSync(dir).forEach(f => {
        let dirPath = path.join(dir, f);
        let isDirectory = fs.statSync(dirPath).isDirectory();
        isDirectory ? walkDir(dirPath, callback) : callback(dirPath);
    });
}

function replaceClasses(content, regex, replacement) {
    return content.replace(regex, replacement);
}

walkDir(srcDir, (filePath) => {
    if (filePath.endsWith('.vue')) {
        let content = fs.readFileSync(filePath, 'utf8');
        let original = content;

        // We only want to process Dashboards and layout components, skipping strictly auth views we already did.
        if (!filePath.includes('LoginView') && !filePath.includes('RegisterView') && !filePath.includes('Forgot') && !filePath.includes('Reset')) {

            // --- Global structure & Main views ---
            content = replaceClasses(content, /bg-slate-50\b(?!(\/| dark:|\w))/g, 'bg-slate-50 dark:bg-slate-950');
            // Sidebars and inner layout wrappers
            content = replaceClasses(content, /bg-white\b(?!(\/| dark:|\w))/g, 'bg-white dark:bg-slate-900');

            // Header borders
            content = replaceClasses(content, /border-slate-100\b(?!(\/| dark:|\w))/g, 'border-slate-100 dark:border-slate-800/80');
            content = replaceClasses(content, /border-slate-200\b(?!(\/| dark:|\w))/g, 'border-slate-200 dark:border-slate-700');

            // Text headings
            content = replaceClasses(content, /text-slate-900\b(?!(\/| dark:|\w))/g, 'text-slate-900 dark:text-white');
            content = replaceClasses(content, /text-slate-800\b(?!(\/| dark:|\w))/g, 'text-slate-800 dark:text-slate-200');

            // Hover states
            content = replaceClasses(content, /hover:bg-slate-50\b(?!(\/| dark:|\w))/g, 'hover:bg-slate-50 dark:hover:bg-slate-800/50');
            content = replaceClasses(content, /hover:bg-slate-100\b(?!(\/| dark:|\w))/g, 'hover:bg-slate-100 dark:hover:bg-slate-800');

            // Input fields embedded in headers or sidebars
            content = replaceClasses(content, /bg-white border-slate-200/g, 'bg-white border-slate-200 dark:bg-slate-900 dark:border-slate-700');
        }

        // --- Write Changes ---
        if (content !== original) {
            fs.writeFileSync(filePath, content, 'utf8');
            console.log(`Upgraded premium dark theme for internal component ${path.basename(filePath)}`);
        }
    }
});

console.log('✅ Dashboard Premium Dark Mode script finished.');
