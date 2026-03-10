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

const isDarkRegex = /:class="settingsStore\.isDark\s*\?\s*'([^']+)'\s*:\s*'([^']+)'"/g;
const isDarkMultilineRegex = /:class="settingsStore\.isDark\s*\n\s*\?\s*'([^']+)'\s*\n\s*:\s*'([^']+)'"/g;

walkDir(srcDir, (filePath) => {
    if (filePath.endsWith('.vue') || filePath.endsWith('.js')) {
        let content = fs.readFileSync(filePath, 'utf8');
        let original = content;

        // 1. Remove single-line comments // that are not in URLs (http:// or https://)
        content = content.replace(/(?<!https?:|http:)\/\/[^\n]*$/gm, '');

        // 2. Remove block comments /* */
        content = content.replace(/\/\*[\s\S]*?\*\//g, '');

        // 3. Convert :class="settingsStore.isDark ? 'darkClasses' : 'lightClasses'" to :class="'lightClasses dark:darkClasses'"
        // By keeping it as :class, Vue automatically merges it with any existing static class="..." attribute!
        const replaceIsDark = (match, darkClasses, lightClasses) => {
            const darkParts = darkClasses.split(' ').filter(Boolean).map(c => `dark:${c}`).join(' ');
            return `:class="'${lightClasses} ${darkParts}'"`;
        };

        content = content.replace(isDarkRegex, replaceIsDark);
        content = content.replace(isDarkMultilineRegex, replaceIsDark);

        // Manual fix for ChatBox.vue
        if (filePath.endsWith('ChatBox.vue')) {
            content = content.replace(
                /:class="isInternalMode\s*\n\s*\?\s*'([^']+)'\s*\n\s*:\s*settingsStore\.isDark\s*\?\s*'([^']+)'\s*:\s*'([^']+)'"/g,
                (match, internal, darkClasses, lightClasses) => {
                    const darkParts = darkClasses.split(' ').filter(Boolean).map(c => `dark:${c}`).join(' ');
                    return `:class="isInternalMode ? '${internal}' : '${lightClasses} ${darkParts}'"`;
                }
            );

            // Amber colors in ChatBox
            content = content.replace(/bg-amber-500\/5/g, 'bg-amber-50 dark:bg-amber-900/20');
            content = content.replace(/border-amber-500\/20/g, 'border-amber-200 dark:border-amber-800/30');
            content = content.replace(/bg-amber-500/g, 'bg-amber-500 dark:bg-amber-600');
            content = content.replace(/text-amber-400\/70/g, 'text-amber-700/70 dark:text-amber-400/70');
            content = content.replace(/text-amber-200/g, 'text-amber-800 dark:text-amber-200');
            content = content.replace(/text-amber-500\/40/g, 'text-amber-600/60 dark:text-amber-500/60');
            content = content.replace(/text-amber-400/g, 'text-amber-600 dark:text-amber-400');
        }

        // 4. Amber colors globally
        if (filePath.endsWith('OperatorDashboard.vue') || filePath.endsWith('AdminDashboard.vue')) {
            content = content.replace(/bg-amber-500\/20 text-amber-400/g, 'bg-amber-100 dark:bg-amber-900/40 text-amber-700 dark:text-amber-400');
        }

        if (filePath.endsWith('StatusBadge.vue')) {
            content = content.replace(/text-amber-600 border border-amber-100/g, 'text-amber-700 dark:text-amber-400 border border-amber-200 dark:border-amber-800/40');
            content = content.replace(/bg-amber-50/g, 'bg-amber-50 dark:bg-amber-900/20');
        }

        if (filePath.endsWith('NotificationToast.vue')) {
            content = content.replace(/bg-amber-500\/10 border-amber-500\/20 text-amber-400/g, 'bg-amber-50 dark:bg-amber-900/20 border-amber-200 dark:border-amber-800/40 text-amber-700 dark:text-amber-400');
        }

        // Clean up empty lines created by comment removal
        content = content.replace(/^\s*[\r\n]/gm, '\n');

        if (content !== original) {
            fs.writeFileSync(filePath, content, 'utf8');
            console.log(`Updated ${filePath}`);
        }
    }
});
