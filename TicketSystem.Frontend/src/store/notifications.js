import { defineStore } from 'pinia';
export const useNotificationStore = defineStore('notifications', {
    state: () => ({
        notifications: []
    }),
    actions: {
        addNotification(message, type = 'info', duration = 3000) {
            const id = Date.now() + Math.random();
            this.notifications.push({ id, message, type, duration });
            if (duration > 0) {
                setTimeout(() => {
                    this.removeNotification(id);
                }, duration);
            }
            return id;
        },
        removeNotification(id) {
            this.notifications = this.notifications.filter(n => n.id !== id);
        },
        success(message, duration) {
            return this.addNotification(message, 'success', duration);
        },
        error(message, duration) {
            return this.addNotification(message, 'error', duration);
        },
        warning(message, duration) {
            return this.addNotification(message, 'warning', duration);
        },
        info(message, duration) {
            return this.addNotification(message, 'info', duration);
        }
    }
});
