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

walkDir(srcDir, (filePath) => {
    if (filePath.endsWith('.vue')) {
        let content = fs.readFileSync(filePath, 'utf8');
        let original = content;

        let changed = true;
        while (changed) {
            let newContent = content.replace(/class="([^"]*)"(\s*(?:\n\s*)?)class="([^"]*)"/g, 'class="$1 $3"');
            if (newContent === content) {
                changed = false;
            }
            content = newContent;
        }

        if (content !== original) {
            fs.writeFileSync(filePath, content, 'utf8');
            console.log(`Merged duplicate classes in ${filePath}`);
        }
    }
});
