<template>
 <div class="fixed bottom-6 right-6 z-[100] flex flex-col gap-3 pointer-events-none">
 <TransitionGroup 
 name="toast" 
 tag="div" 
 class="flex flex-col gap-3"
 >
 <div 
 v-for="notification in notificationStore.notifications" 
 :key="notification.id"
 class="pointer-events-auto flex items-center gap-3 px-6 py-4 rounded-2xl shadow-md border min-w-[320px] max-w-md animate-in slide-in-from-right-10 duration-300"
 :class="[
 notification.type === 'success' ? 'bg-emerald-500/10 border-emerald-500/20 text-emerald-400' :
 notification.type === 'error' ? 'bg-rose-500/10 border-rose-500/20 text-rose-400' :
 notification.type === 'warning' ? 'bg-amber-50 dark:bg-amber-900/20 border-amber-200 dark:border-amber-800/40 text-amber-700 dark:text-amber-400' :
 'bg-blue-500/10 border-blue-500/20 text-blue-400'
 ]"
 >
 <div class="flex-shrink-0">
 <CheckCircleIcon v-if="notification.type === 'success'" class="w-6 h-6" />
 <AlertCircleIcon v-else-if="notification.type === 'error'" class="w-6 h-6" />
 <AlertTriangleIcon v-else-if="notification.type === 'warning'" class="w-6 h-6" />
 <InfoIcon v-else class="w-6 h-6" />
 </div>
 <div class="flex-1 text-sm font-semibold tracking-wide">
 {{ notification.message }}
 </div>
 <button 
 @click="notificationStore.removeNotification(notification.id)"
 class="flex-shrink-0 p-1 hover:bg-slate-50 dark:bg-slate-950 rounded-lg transition-colors"
 >
 <XIcon class="w-4 h-4 opacity-50 hover:opacity-100" />
 </button>
 </div>
 </TransitionGroup>
 </div>
</template>
<script setup>
import { useNotificationStore } from '../store/notifications';
import { 
 CheckCircleIcon, 
 AlertCircleIcon, 
 AlertTriangleIcon, 
 InfoIcon,
 XIcon 
} from 'lucide-vue-next';
const notificationStore = useNotificationStore();
</script>
<style scoped>
.toast-enter-active,
.toast-leave-active {
 transition: all 0.4s cubic-bezier(0.16, 1, 0.3, 1);
}
.toast-enter-from {
 opacity: 0;
 transform: translateX(40px) scale(0.9);
}
.toast-leave-to {
 opacity: 0;
 transform: translateX(20px) scale(0.9);
}
</style>
