const fs = require('fs');
const path = require('path');

const srcDir = path.join(__dirname, 'src', 'views');

function fixAuth(file) {
    const filePath = path.join(srcDir, file);
    if (!fs.existsSync(filePath)) return;
    let content = fs.readFileSync(filePath, 'utf8');

    // Hardcode the background of the screen
    content = content.replace(/bg-slate-50 dark:bg-slate-[^\s"']+/g, 'bg-slate-50 dark:bg-[#020617]');

    // Hardcode the background of the cards
    content = content.replace(/bg-white\/90 dark:bg-slate-[^\s"']+/g, 'bg-white/90 dark:bg-[#0f172a]');

    // Also fix Borders
    content = content.replace(/border-slate-200\/60 dark:border-slate-[^\s"']+/g, 'border-slate-200/60 dark:border-[#1e293b]');

    // Inputs
    content = content.replace(/bg-slate-50\/50 dark:bg-slate-[^\s"']+/g, 'bg-slate-50/50 dark:bg-[#1e293b]');

    // Blobs
    content = content.replace(/dark:bg-blue-600\/10/g, 'dark:bg-blue-500/10');
    content = content.replace(/dark:bg-indigo-600\/10/g, 'dark:bg-indigo-500/10');

    fs.writeFileSync(filePath, content, 'utf8');
}

['LoginView.vue', 'RegisterView.vue', 'ForgotPasswordView.vue', 'ResetPasswordView.vue'].forEach(fixAuth);
console.log('Fixed Auth Views to be very dark.');
