<template>
  <div class="flex flex-col w-full h-full"
    :class="settingsStore.isDark ? 'bg-slate-900' : 'bg-white'">

    <!-- Messages area -->
    <div ref="messagesContainer" class="flex-1 overflow-y-auto space-y-2 mb-3 custom-scrollbar px-1">

      <div v-if="messages.length === 0"
        class="flex flex-col items-center justify-center h-48 space-y-3">
        <MessageSquareIcon class="w-8 h-8 opacity-20"
          :class="settingsStore.isDark ? 'text-slate-500' : 'text-slate-400'" />
        <p class="text-xs font-bold uppercase tracking-widest"
          :class="settingsStore.isDark ? 'text-slate-600' : 'text-slate-400'">
          Sin actividad aún
        </p>
      </div>

      <div v-for="msg in messages" :key="msg.id"
        :class="esPropio(msg) ? 'flex justify-end' : 'flex justify-start'">

        <!-- Nota interna -->
        <div v-if="msg.interno"
          class="w-full max-w-sm rounded-xl border border-amber-500/20 bg-amber-500/5 px-3 py-2 relative">
          <span class="absolute -top-2 right-2 bg-amber-500 text-[8px] font-black text-black px-2 py-0.5 rounded-full uppercase tracking-wider">
            Nota Interna
          </span>
          <p class="text-[10px] font-semibold text-amber-400/70 mb-1">{{ msg.autor }}</p>
          <p class="text-sm text-amber-200">{{ msg.texto }}</p>
          <p class="text-[9px] text-amber-500/40 mt-1 font-mono">{{ formatTime(msg.timestamp) }}</p>
        </div>

        <!-- Mensaje propio -->
        <div v-else-if="esPropio(msg)"
          class="max-w-xs px-4 py-2.5 rounded-2xl rounded-tr-sm bg-blue-600 text-white">
          <p class="text-[10px] font-semibold opacity-60 mb-1">{{ msg.autor }}</p>
          <p class="text-sm leading-relaxed">{{ msg.texto }}</p>
          <p class="text-[9px] opacity-40 mt-1 font-mono text-right">{{ formatTime(msg.timestamp) }}</p>
        </div>

        <!-- Mensaje ajeno -->
        <div v-else
          class="max-w-xs px-4 py-2.5 rounded-2xl rounded-tl-sm border"
          :class="settingsStore.isDark
            ? 'bg-slate-800 border-slate-700 text-white'
            : 'bg-slate-50 border-slate-200 text-slate-900'">
          <p class="text-[10px] font-semibold opacity-60 mb-1">
            {{ msg.autor }}
            <span class="opacity-40 uppercase tracking-widest text-[8px] ml-1">/ {{ roleLabel(msg.autorRol) }}</span>
          </p>
          <p class="text-sm leading-relaxed">{{ msg.texto }}</p>
          <p class="text-[9px] opacity-30 mt-1 font-mono">{{ formatTime(msg.timestamp) }}</p>
        </div>

      </div>
    </div>

    <!-- Input area -->
    <div class="space-y-2 flex-shrink-0">

      <!-- Toggle nota interna -->
      <div v-if="authStore.isAdmin || authStore.isOperador" class="flex items-center gap-2 px-1">
        <button @click="isInternalMode = !isInternalMode"
          class="flex items-center gap-2 group">
          <div class="w-7 h-3.5 rounded-full relative transition-all"
            :class="isInternalMode
              ? 'bg-amber-500'
              : settingsStore.isDark ? 'bg-slate-700' : 'bg-slate-200'">
            <div class="absolute top-0.5 w-2.5 h-2.5 rounded-full bg-white transition-all shadow-sm"
              :class="isInternalMode ? 'left-[calc(100%-0.75rem)]' : 'left-0.5'"></div>
          </div>
          <span class="text-[10px] font-black uppercase tracking-widest transition-colors"
            :class="isInternalMode
              ? 'text-amber-400'
              : settingsStore.isDark ? 'text-slate-600' : 'text-slate-400'">
            Nota Interna
          </span>
        </button>
      </div>

      <!-- Input -->
      <div class="flex gap-2">
        <input v-model="newMessage" @keyup.enter="sendMessage"
          :placeholder="isInternalMode ? 'Nota interna...' : 'Escribe un mensaje...'"
          class="flex-1 border px-4 py-3 rounded-xl text-sm outline-none transition-all font-medium"
          :class="settingsStore.isDark
            ? 'bg-slate-800 border-slate-700 text-white placeholder-slate-600 focus:border-blue-500'
            : 'bg-slate-50 border-slate-200 text-slate-900 placeholder-slate-400 focus:border-blue-400'" />
        <button @click="sendMessage" :disabled="!newMessage.trim()"
          class="px-5 py-2 text-white rounded-xl text-xs font-black uppercase tracking-widest transition-all active:scale-95 disabled:opacity-30 disabled:cursor-not-allowed"
          :class="isInternalMode ? 'bg-amber-500 hover:bg-amber-400' : 'bg-blue-600 hover:bg-blue-500'">
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
import { useSettingsStore } from '../store/settings';

const props = defineProps({ ticketId: String });

const ticketsStore   = useTicketsStore();
const authStore      = useAuthStore();
const settingsStore  = useSettingsStore();

const messages          = ref([]);
const newMessage        = ref('');
const isInternalMode    = ref(false);
const messagesContainer = ref(null);

const formatTime = (timestamp) => {
  if (!timestamp) return '';
  try {
    const date = new Date(timestamp);
    if (isNaN(date.getTime())) return '';
    return new Intl.DateTimeFormat('es-AR', {
      hour: '2-digit', minute: '2-digit',
      day: '2-digit', month: '2-digit',
      timeZone: 'America/Argentina/Buenos_Aires'
    }).format(date);
  } catch { return ''; }
};

const roleLabel = (r) => {
  if (r === null || r === undefined) return '';
  const n = typeof r === 'string' ? parseInt(r, 10) : r;
  return n === 2 ? 'Admin' : n === 1 ? 'Operador' : 'Usuario';
};

const esPropio = (msg) => msg.autorId === authStore.user?.id;

const mapComments = (comments) => comments.map(c => ({
  id: c.id,
  autorId: c.autorId,
  autor: c.autor || 'Sistema',
  autorRol: c.autorRol ?? c.rol ?? null,
  texto: c.mensaje,
  interno: c.interno,
  timestamp: c.fecha ?? null
}));

watch(() => props.ticketId, async (id) => {
  if (id) {
    await ticketsStore.fetchComments(id);
    messages.value = mapComments(ticketsStore.comments);
  } else {
    messages.value = [];
  }
}, { immediate: true });

watch(() => ticketsStore.comments, (c) => {
  if (props.ticketId) messages.value = mapComments(c);
}, { deep: true });

watch(messages, () => {
  nextTick(() => {
    if (messagesContainer.value)
      messagesContainer.value.scrollTop = messagesContainer.value.scrollHeight;
  });
});

const sendMessage = async () => {
  if (!newMessage.value.trim() || !props.ticketId) return;
  try {
    await ticketsStore.addComment(props.ticketId, { mensaje: newMessage.value.trim(), interno: isInternalMode.value });
  } catch (e) { console.error(e); }
  newMessage.value = '';
};
</script>

<style scoped>
.custom-scrollbar::-webkit-scrollbar { width: 4px; }
.custom-scrollbar::-webkit-scrollbar-track { background: transparent; }
.custom-scrollbar::-webkit-scrollbar-thumb { background: rgba(100,100,100,0.2); border-radius: 10px; }
</style>