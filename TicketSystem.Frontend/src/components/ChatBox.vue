<template>
 <div class="japanese-box flex flex-col w-full max-w-full h-full max-h-[60vh] bg-white">
 <h3 class="text-lg font-black text-slate-900 mb-4">Chat de Soporte</h3>

 <!-- messages area -->
 <div ref="messagesContainer" class="flex-1 overflow-y-auto space-y-3 mb-4 pb-2 custom-scrollbar">
 <div
 v-for="msg in messages"
 :key="msg.id"
 :class="{
 'flex justify-end': msg.rol === 'usuario',
 'flex justify-start': msg.rol !== 'usuario'
 }"
 >
 <div
 :class="{
 'bg-blue-600 text-white rounded-l-2xl rounded-tr-2xl shadow-lg shadow-blue-200': msg.rol === 'usuario' && !msg.interno,
 'bg-white text-slate-900 rounded-r-2xl rounded-tl-2xl border border-slate-200': msg.rol !== 'usuario' && !msg.interno,
 'bg-amber-500/10 text-amber-200 rounded-2xl border border-amber-500/20 w-full max-w-sm': msg.interno
 }"
 class="px-4 py-2 relative group"
 >
 <div v-if="msg.interno" class="absolute -top-2 -right-2 bg-amber-500 text-[8px] font-black text-slate-950 px-2 py-0.5 rounded-full uppercase tracking-tighter shadow-lg">Nota Interna</div>
 
 <p class="text-[10px] font-bold mb-1 opacity-70 flex items-center gap-2">
 {{ msg.autor }} 
 <span class="text-[9px] opacity-50 tracking-widest uppercase">/ {{ roleLabel(msg.autorRol) }}</span>
 </p>
 <p class="text-sm leading-relaxed">{{ msg.texto }}</p>
 <p class="text-[9px] opacity-40 mt-1 font-mono tracking-tighter">{{ formatTime(msg.timestamp) }}</p>
 </div>
 </div>
 <div v-if="messages.length === 0" class="flex flex-col items-center justify-center h-48 text-slate-600 space-y-4">
 <div class="w-12 h-12 rounded-full border border-slate-200 flex items-center justify-center bg-white/[0.02]">
 <MessageSquareIcon class="w-6 h-6 opacity-20" />
 </div>
 <p class="text-xs font-bold uppercase tracking-widest">Sin actividad aún</p>
 </div>
 </div>

 <!-- input area -->
 <div class="space-y-3">
 <div v-if="authStore.isAdmin || authStore.isOperador" class="flex items-center gap-4 px-2">
 <label class="flex items-center gap-2 cursor-pointer group">
 <div 
 class="w-8 h-4 rounded-full transition-all relative border border-slate-200"
 :class="isInternalMode ? 'bg-amber-500/20 border-amber-500/40' : 'bg-white'"
 >
 <input type="checkbox" v-model="isInternalMode" class="hidden" />
 <div 
 class="absolute top-0.5 w-2.5 h-2.5 rounded-full transition-all shadow-sm"
 :class="isInternalMode ? 'right-0.5 bg-amber-400' : 'left-0.5 bg-slate-500'"
 ></div>
 </div>
 <span class="text-[10px] font-black uppercase tracking-widest group-hover:text-amber-400 transition-colors" :class="isInternalMode ? 'text-amber-400' : 'text-slate-500'">Modo Nota Interna</span>
 </label>
 </div>

 <div class="flex gap-2">
 <input
 v-model="newMessage"
 @keyup.enter="sendMessage"
 :placeholder="isInternalMode ? 'Escribe una nota para el equipo...' : 'Escribe tu mensaje...'"
 class="flex-1 bg-white/[0.02] border border-slate-200 px-6 py-4 rounded-2xl text-sm placeholder-slate-600 focus:border-blue-500/50 outline-none transition-all font-medium"
 />
 <button
 @click="sendMessage"
 :disabled="!newMessage.trim()"
 :class="isInternalMode ? 'bg-amber-600 hover:bg-amber-500 shadow-amber-500/20' : 'bg-blue-600 hover:bg-blue-500 shadow-blue-200'"
 class="px-8 py-2 text-white rounded-2xl text-xs font-black uppercase tracking-widest disabled:opacity-20 disabled:cursor-not-allowed transition-all active:scale-95 shadow-sm"
 >
 {{ isInternalMode ? 'Nota' : 'Enviar' }}
 </button>
 </div>
 </div>
 </div>
</template>

<script setup>
import { ref, watch, nextTick } from 'vue';
import { MessageSquareIcon } from 'lucide-vue-next';
import { useTicketsStore } from '../store/tickets';
import { useAuthStore } from '../store/auth';

const props = defineProps({
 ticketId: String
});

const ticketsStore = useTicketsStore();
const authStore = useAuthStore();

const messages = ref([]);
const newMessage = ref('');
const isInternalMode = ref(false);
const messagesContainer = ref(null);

const formatTime = (timestamp) => {
 if (!timestamp) return '';
 const date = timestamp instanceof Date ? timestamp : new Date(timestamp);
 if (isNaN(date)) return '';
 try {
 return new Intl.DateTimeFormat('es-AR', {
 hour: '2-digit',
 minute: '2-digit',
 timeZone: 'America/Argentina/Buenos_Aires'
 }).format(date);
 } catch (e) {
 return date.toLocaleTimeString('es-ES', { hour: '2-digit', minute: '2-digit' });
 }
};

const roleLabel = (r) => {
 if (r === null || r === undefined) return '';
 const n = typeof r === 'string' ? parseInt(r, 10) : r;
 return n === 2 ? 'Administrador' : n === 1 ? 'Operador' : 'Usuario';
};

// whenever ticketId changes, load comments from backend
watch(
 () => props.ticketId,
 async (id) => {
 if (id) {
  await ticketsStore.fetchComments(id);
  messages.value = ticketsStore.comments.map(c => {
    return {
      id: c.id,
      rol: c.interno ? 'operador' : 'usuario',
      autor: c.autor || 'Sistema',
      autorRol: c.autorRol ?? c.rol ?? c.Rol ?? null,
      texto: c.mensaje,
      timestamp: c.fecha ? new Date(c.fecha).getTime() : Date.now()
    };
  });
  } else {
 messages.value = [];
 }
 },
 { immediate: true }
);

// keep messages synced if store updates (e.g. other components added comments)
watch(
 () => ticketsStore.comments,
 (newComments) => {
 if (props.ticketId) {
  messages.value = newComments.map(c => {
    return {
      id: c.id,
      rol: c.interno ? 'operador' : 'usuario',
      autor: c.autor || 'Sistema',
      autorRol: c.autorRol ?? c.rol ?? c.Rol ?? null,
      texto: c.mensaje,
      timestamp: c.fecha ? new Date(c.fecha).getTime() : Date.now()
    };
  });
  }
  },
 { deep: true }
);

// auto-scroll when messages update
watch(messages, () => {
 nextTick(() => {
 if (messagesContainer.value) {
 messagesContainer.value.scrollTop = messagesContainer.value.scrollHeight;
 }
 });
});


const sendMessage = async () => {
 if (!newMessage.value.trim() || !props.ticketId) return;
 const texto = newMessage.value.trim();
  const interno = isInternalMode.value;
  try {
  await ticketsStore.addComment(props.ticketId, { mensaje: texto, interno });
  // store watcher will update messages automatically
 } catch (err) {
 console.error('Failed to send chat message', err);
 }
 newMessage.value = '';
};
</script>

<style scoped>
.custom-scrollbar::-webkit-scrollbar {
 width: 6px;
}
.custom-scrollbar::-webkit-scrollbar-track {
 background: transparent;
}
.custom-scrollbar::-webkit-scrollbar-thumb {
 background: rgba(255, 255, 255, 0.1);
 border-radius: 10px;
}
.custom-scrollbar::-webkit-scrollbar-thumb:hover {
 background: rgba(100, 100, 100, 0.4);
}
</style>
