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
    let newContent = content.replace(regex, replacement);
    // Simple deduplication rule if replacement added something already there
    return newContent;
}

walkDir(srcDir, (filePath) => {
    if (filePath.endsWith('.vue')) {
        let content = fs.readFileSync(filePath, 'utf8');
        let original = content;

        // --- Premium Auth Views (Login, Register, Forgot, Reset) ---
        if (filePath.includes('LoginView') || filePath.includes('RegisterView') || filePath.includes('Forgot') || filePath.includes('Reset')) {
            // Background
            content = replaceClasses(content, /bg-slate-50\b(?!(\/| dark:|\w))/g, 'bg-slate-50 dark:bg-slate-950');
            // Main Form Card
            content = replaceClasses(content, /bg-white\/90\b(?!(\/| dark:|\w))/g, 'bg-white/90 dark:bg-slate-900/90');
            content = replaceClasses(content, /bg-white\b(?!(\/| dark:|\w))/g, 'bg-white dark:bg-slate-900');
            // Borders
            content = replaceClasses(content, /border-slate-200\/60\b(?!(\/| dark:|\w))/g, 'border-slate-200/60 dark:border-slate-800');
            content = replaceClasses(content, /border-slate-200\/80\b(?!(\/| dark:|\w))/g, 'border-slate-200/80 dark:border-slate-800');
            content = replaceClasses(content, /border-slate-200\b(?!(\/| dark:|\w))/g, 'border-slate-200 dark:border-slate-800');
            // Text Headings & Body
            content = replaceClasses(content, /text-slate-900\b(?!(\/| dark:|\w))/g, 'text-slate-900 dark:text-white');
            // Input Fields
            content = replaceClasses(content, /bg-slate-50\/50\b(?!(\/| dark:|\w))/g, 'bg-slate-50/50 dark:bg-slate-800/50');
            // Blobs opacity reduction in dark mode for a sleeker look
            content = replaceClasses(content, /bg-blue-600\/5\b(?!(\/| dark:|\w))/g, 'bg-blue-600/5 dark:bg-blue-600/10');
            content = replaceClasses(content, /bg-indigo-600\/5\b(?!(\/| dark:|\w))/g, 'bg-indigo-600/5 dark:bg-indigo-600/10');
        }

        // --- Write Changes ---
        if (content !== original) {
            fs.writeFileSync(filePath, content, 'utf8');
            console.log(`Upgraded premium dark theme for ${path.basename(filePath)}`);
        }
    }
});

console.log('✅ Premium Dark Mode script finished.');
