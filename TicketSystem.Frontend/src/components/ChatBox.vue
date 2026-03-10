<template>
  <div class="flex flex-col w-full h-full"
    :class="'bg-white dark:bg-slate-900'">
    <div ref="messagesContainer"
      class="flex-1 overflow-y-auto space-y-2 mb-3 custom-scrollbar px-1">
      <div v-if="messages.length === 0"
        class="flex flex-col items-center justify-center h-48 space-y-3">
        <MessageSquareIcon class="w-8 h-8 opacity-20"
          :class="'text-slate-400 dark:text-slate-500'" />
        <p class="text-xs font-bold uppercase tracking-widest"
          :class="'text-slate-400 dark:text-slate-600'">
          Sin actividad aún
        </p>
      </div>
      <div v-for="(msg, index) in messages" :key="msg.id">
        <div v-if="showDateSeparator(index)" class="flex items-center gap-3 my-3">
          <div class="flex-1 h-px" :class="'bg-slate-100 dark:bg-slate-800'"></div>
          <span class="text-[9px] font-black uppercase tracking-widest"
            :class="'text-slate-400 dark:text-slate-600'">
            {{ formatDateSeparator(msg.timestamp) }}
          </span>
          <div class="flex-1 h-px" :class="'bg-slate-100 dark:bg-slate-800'"></div>
        </div>
        <div :class="esPropio(msg) ? 'flex justify-end' : 'flex justify-start'">
          <div v-if="msg.interno"
            class="w-full max-w-sm rounded-xl border border-amber-200 dark:border-amber-800/30 bg-amber-50 dark:bg-amber-900/20 px-3 py-2 relative mt-2">
            <span class="absolute -top-2 right-2 bg-amber-500 dark:bg-amber-600 text-[8px] font-black text-black px-2 py-0.5 rounded-full uppercase tracking-wider">
              Nota Interna
            </span>
            <p class="text-[10px] font-semibold text-amber-700/70 dark:text-amber-400/70 mb-1">{{ msg.autor }}</p>
            <p class="text-sm text-amber-800 dark:text-amber-200">{{ msg.texto }}</p>
            <div v-if="msg.adjuntos?.length" class="mt-2 space-y-1">
              <a v-for="adj in msg.adjuntos" :key="adj.id"
                :href="resolveUrl(adj.url)" target="_blank"
                class="flex items-center gap-2 text-xs text-amber-700 dark:text-amber-300 hover:underline">
                <PaperclipIcon class="w-3 h-3 flex-shrink-0" />
                <span class="truncate max-w-[180px]">{{ adj.nombreOriginal }}</span>
              </a>
            </div>
            <p class="text-[9px] text-amber-600/60 dark:text-amber-500/60 mt-1 font-mono">{{ formatTime(msg.timestamp) }}</p>
          </div>
          <div v-else-if="esPropio(msg)" class="flex flex-col items-end gap-0.5 max-w-xs">
            <div class="px-4 py-2.5 rounded-2xl rounded-tr-sm bg-blue-600 text-white">
              <p class="text-sm leading-relaxed">{{ msg.texto }}</p>
              <div v-if="msg.adjuntos?.length" class="mt-2 space-y-1">
                <a v-for="adj in msg.adjuntos" :key="adj.id"
                  :href="resolveUrl(adj.url)" target="_blank"
                  class="flex items-center gap-2 text-xs text-blue-100 hover:text-white hover:underline">
                  <PaperclipIcon class="w-3 h-3 flex-shrink-0" />
                  <span class="truncate max-w-[180px]">{{ adj.nombreOriginal }}</span>
                </a>
              </div>
            </div>
            <p class="text-[9px] font-mono px-1" :class="'text-slate-400 dark:text-slate-600'">
              {{ formatTime(msg.timestamp) }}
            </p>
          </div>
          <div v-else class="flex flex-col items-start gap-0.5 max-w-xs">
            <p class="text-[10px] font-semibold px-1" :class="'text-slate-400 dark:text-slate-500'">
              {{ msg.autor }}
              <span class="uppercase tracking-widest text-[8px] ml-1 px-1.5 py-0.5 rounded-full"
                :class="roleBadgeClass(msg.autorRol)">
                {{ roleLabel(msg.autorRol) }}
              </span>
            </p>
            <div class="px-4 py-2.5 rounded-2xl rounded-tl-sm border"
              :class="'bg-slate-50 dark:bg-slate-800 border-slate-200 dark:border-slate-700 text-slate-900 dark:text-white'">
              <p class="text-sm leading-relaxed">{{ msg.texto }}</p>
              <div v-if="msg.adjuntos?.length" class="mt-2 space-y-1">
                <a v-for="adj in msg.adjuntos" :key="adj.id"
                  :href="resolveUrl(adj.url)" target="_blank"
                  class="flex items-center gap-2 text-xs text-blue-500 hover:underline">
                  <PaperclipIcon class="w-3 h-3 flex-shrink-0" />
                  <span class="truncate max-w-[180px]">{{ adj.nombreOriginal }}</span>
                </a>
              </div>
            </div>
            <p class="text-[9px] font-mono px-1" :class="'text-slate-400 dark:text-slate-600'">
              {{ formatTime(msg.timestamp) }}
            </p>
          </div>
        </div>
      </div>
      <div v-if="alguienEscribe" class="flex justify-start">
        <div class="px-4 py-3 rounded-2xl rounded-tl-sm border"
          :class="'bg-slate-50 dark:bg-slate-800 border-slate-200 dark:border-slate-700'">
          <div class="flex gap-1 items-center">
            <span v-for="i in 3" :key="i"
              class="w-1.5 h-1.5 rounded-full bg-slate-400 animate-bounce"
              :style="`animation-delay: ${(i-1)*0.2}s`"></span>
          </div>
        </div>
      </div>
    </div>

    <div class="space-y-2 flex-shrink-0 border-t pt-3"
      :class="'border-slate-100 dark:border-slate-800'">

      <div v-if="authStore.isAdmin || authStore.isOperador"
        class="flex items-center gap-2 px-1">
        <button @click="isInternalMode = !isInternalMode" class="flex items-center gap-2 group">
          <div class="w-7 h-3.5 rounded-full relative transition-all"
            :class="isInternalMode ? 'bg-amber-500 dark:bg-amber-600' : 'bg-slate-200 dark:bg-slate-700'">
            <div class="absolute top-0.5 w-2.5 h-2.5 rounded-full bg-white dark:bg-slate-900 transition-all shadow-sm"
              :class="isInternalMode ? 'left-[calc(100%-0.75rem)]' : 'left-0.5'"></div>
          </div>
          <span class="text-[10px] font-black uppercase tracking-widest transition-colors"
            :class="isInternalMode ? 'text-amber-600 dark:text-amber-400' : 'text-slate-400 dark:text-slate-600'">
            Nota Interna
          </span>
        </button>
      </div>

      <div v-if="pendingFiles.length > 0" class="flex flex-wrap gap-2 px-1">
        <div v-for="(f, i) in pendingFiles" :key="i"
          class="flex items-center gap-1.5 text-xs px-2 py-1 rounded-lg border"
          :class="'bg-slate-50 dark:bg-slate-800 border-slate-200 dark:border-slate-700 text-slate-600 dark:text-slate-300'">
          <PaperclipIcon class="w-3 h-3 flex-shrink-0" />
          <span class="max-w-[100px] truncate">{{ f.name }}</span>
          <button @click="removeFile(i)" class="text-slate-400 hover:text-rose-400 transition-colors ml-0.5">
            <XIcon class="w-3 h-3" />
          </button>
        </div>
      </div>

      <div class="flex gap-2 items-end">
        <textarea
          ref="inputRef"
          v-model="newMessage"
          @keydown.enter.exact.prevent="sendMessage"
          @keydown.enter.shift.exact="newMessage += '\n'"
          @input="autoResize(); notificarEscribiendo()"
          rows="1"
          :placeholder="isInternalMode ? 'Nota interna...' : 'Escribe un mensaje...'"
          class="flex-1 border px-4 py-3 rounded-xl text-sm outline-none transition-all font-medium resize-none"
          :class="'bg-slate-50 dark:bg-slate-800 border-slate-200 dark:border-slate-700 text-slate-900 dark:text-white placeholder-slate-400 focus:border-blue-400 dark:focus:border-blue-500'"
          style="max-height: 120px; overflow-y: auto;" />

        <input ref="fileInputRef" type="file" multiple class="hidden"
          accept="image/*,.pdf,.doc,.docx,.xls,.xlsx,.txt"
          @change="onFileSelected" />

        <button @click="fileInputRef?.click()"
          class="p-3 rounded-xl border transition-all flex-shrink-0"
          :class="pendingFiles.length > 0
            ? 'bg-blue-50 dark:bg-blue-900/20 border-blue-400 text-blue-500'
            : 'bg-slate-50 dark:bg-slate-800 border-slate-200 dark:border-slate-700 text-slate-400 hover:text-blue-500 hover:border-blue-400'">
          <PaperclipIcon class="w-4 h-4" />
        </button>

        <button @click="sendMessage"
          :disabled="(!newMessage.trim() && pendingFiles.length === 0) || sending"
          class="px-5 py-3 text-white rounded-xl text-xs font-black uppercase tracking-widest transition-all active:scale-95 disabled:opacity-30 disabled:cursor-not-allowed flex-shrink-0"
          :class="isInternalMode ? 'bg-amber-500 dark:bg-amber-600 hover:bg-amber-400' : 'bg-blue-600 hover:bg-blue-500'">
          <SendIcon v-if="!sending" class="w-4 h-4" />
          <div v-else class="w-4 h-4 border-2 border-white/30 border-t-white rounded-full animate-spin"></div>
        </button>
      </div>

      <div v-if="uploadProgress > 0 && uploadProgress < 100"
        class="h-1 rounded-full overflow-hidden bg-slate-100 dark:bg-slate-800">
        <div class="h-full bg-blue-500 transition-all duration-300 rounded-full"
          :style="`width: ${uploadProgress}%`"></div>
      </div>

      <p class="text-[9px] px-1" :class="'text-slate-300 dark:text-slate-700'">
        Enter para enviar · Shift+Enter para nueva línea
      </p>
    </div>
  </div>
</template>

<script setup>
import { ref, watch, nextTick, onMounted, onUnmounted } from 'vue';
import * as signalR from '@microsoft/signalr';
import { MessageSquareIcon, SendIcon, PaperclipIcon, XIcon } from 'lucide-vue-next';
import { useTicketsStore } from '../store/tickets';
import { useAuthStore, API_URL } from '../store/auth';
import { useSettingsStore } from '../store/settings';
import axios from 'axios';

const props = defineProps({ ticketId: String });
const ticketsStore  = useTicketsStore();
const authStore     = useAuthStore();
const settingsStore = useSettingsStore();

const messages          = ref([]);
const newMessage        = ref('');
const isInternalMode    = ref(false);
const messagesContainer = ref(null);
const inputRef          = ref(null);
const fileInputRef      = ref(null);
const sending           = ref(false);
const alguienEscribe    = ref(false);
const pendingFiles      = ref([]);
const uploadProgress    = ref(0);
let escribiendoTimer    = null;
let connection          = null;

const BACKEND_BASE = (() => {
  const v = import.meta.env.VITE_API_URL;
  if (v) return v.replace(/\/api$/, '');
  return 'http://localhost:5134';
})();

const resolveUrl = (url) => {
  if (!url) return '#';
  if (url.startsWith('http')) return url;
  return `${BACKEND_BASE}${url}`;
};

const parseDate = (timestamp) => {
  if (!timestamp) return null;
  const str = timestamp.toString();
  const normalized = str.endsWith('Z') || str.includes('+') ? str : str + 'Z';
  const d = new Date(normalized);
  return isNaN(d.getTime()) ? null : d;
};

const formatTime = (timestamp) => {
  const date = parseDate(timestamp);
  if (!date) return '';
  return new Intl.DateTimeFormat('es-AR', {
    hour: '2-digit', minute: '2-digit',
    timeZone: 'America/Argentina/Buenos_Aires'
  }).format(date);
};

const formatDateSeparator = (timestamp) => {
  const date = parseDate(timestamp);
  if (!date) return '';
  const now       = new Date();
  const today     = new Date(now.toLocaleDateString('en-CA', { timeZone: 'America/Argentina/Buenos_Aires' }));
  const yesterday = new Date(today);
  yesterday.setDate(yesterday.getDate() - 1);
  const dateDay   = new Date(date.toLocaleDateString('en-CA', { timeZone: 'America/Argentina/Buenos_Aires' }));
  if (dateDay.getTime() === today.getTime())     return 'Hoy';
  if (dateDay.getTime() === yesterday.getTime()) return 'Ayer';
  return new Intl.DateTimeFormat('es-AR', {
    day: '2-digit', month: 'long', year: 'numeric',
    timeZone: 'America/Argentina/Buenos_Aires'
  }).format(date);
};

const showDateSeparator = (index) => {
  if (index === 0) return true;
  const prev = parseDate(messages.value[index - 1]?.timestamp);
  const curr = parseDate(messages.value[index]?.timestamp);
  if (!prev || !curr) return false;
  const tz = 'America/Argentina/Buenos_Aires';
  return prev.toLocaleDateString('en-CA', { timeZone: tz }) !==
         curr.toLocaleDateString('en-CA', { timeZone: tz });
};

const roleLabel = (r) => {
  if (r === null || r === undefined) return '';
  const n = typeof r === 'string' ? parseInt(r, 10) : r;
  return n === 2 ? 'Admin' : n === 1 ? 'Operador' : 'Usuario';
};

const roleBadgeClass = (r) => {
  const n = typeof r === 'string' ? parseInt(r, 10) : r;
  if (n === 2) return 'bg-purple-500/10 text-purple-400';
  if (n === 1) return 'bg-emerald-500/10 text-emerald-400';
  return 'bg-slate-500/10 text-slate-400';
};

const esPropio = (msg) => {
  const userId  = authStore.user?.id?.toString().toLowerCase();
  const autorId = msg.autorId?.toString().toLowerCase();
  return userId && autorId && userId === autorId;
};

const mapComment = (c) => ({
  id:        c.id,
  autorId:   c.autorId,
  autor:     c.autor || 'Sistema',
  autorRol:  c.autorRol ?? c.rol ?? null,
  texto:     c.mensaje,
  interno:   c.interno,
  timestamp: c.fecha ?? null,
  adjuntos:  c.adjuntos ?? []
});

const scrollToBottom = (smooth = false) => {
  nextTick(() => {
    if (!messagesContainer.value) return;
    messagesContainer.value.scrollTo({
      top: messagesContainer.value.scrollHeight,
      behavior: smooth ? 'smooth' : 'instant'
    });
  });
};

const autoResize = () => {
  const el = inputRef.value;
  if (!el) return;
  el.style.height = 'auto';
  el.style.height = Math.min(el.scrollHeight, 120) + 'px';
};

const resetInput = () => {
  newMessage.value = '';
  nextTick(() => {
    if (inputRef.value) {
      inputRef.value.style.height = 'auto';
      inputRef.value.focus();
    }
  });
};

const onFileSelected = (e) => {
  const files = Array.from(e.target.files ?? []);
  pendingFiles.value = [...pendingFiles.value, ...files];
  if (fileInputRef.value) fileInputRef.value.value = '';
};

const removeFile = (i) => {
  pendingFiles.value = pendingFiles.value.filter((_, idx) => idx !== i);
};

const uploadFiles = async (comentarioId) => {
  if (!pendingFiles.value.length) return;
  const total = pendingFiles.value.length;
  for (let i = 0; i < total; i++) {
    const form = new FormData();
    form.append('archivo', pendingFiles.value[i]);
    await axios.post(`${API_URL}/archivos/comentario/${comentarioId}`, form, {
      headers: { 'Content-Type': 'multipart/form-data' }
    });
    uploadProgress.value = Math.round(((i + 1) / total) * 100);
  }
  pendingFiles.value   = [];
  uploadProgress.value = 0;
};

const getToken = () => localStorage.getItem('token') ?? '';

const conectarSignalR = async () => {
  if (!props.ticketId) return;
  connection = new signalR.HubConnectionBuilder()
    .withUrl(`${BACKEND_BASE}/hubs/tickets`, { accessTokenFactory: () => getToken() })
    .withAutomaticReconnect()
    .configureLogging(signalR.LogLevel.Warning)
    .build();

  connection.on('NuevoMensaje', (comentario) => {
    const msg = mapComment(comentario);
    if (!messages.value.find(m => m.id === msg.id)) {
      messages.value.push(msg);
      scrollToBottom(true);
    }
  });

  connection.on('UsuarioEscribiendo', () => {
    alguienEscribe.value = true;
    clearTimeout(escribiendoTimer);
    escribiendoTimer = setTimeout(() => { alguienEscribe.value = false; }, 2500);
  });

  try {
    await connection.start();
    await connection.invoke('UnirseATicket', props.ticketId);
  } catch (e) {
    console.warn('[SignalR] No disponible, usando REST:', e.message);
  }
};

const desconectarSignalR = async () => {
  if (connection) {
    try {
      if (props.ticketId) await connection.invoke('SalirDeTicket', props.ticketId);
      await connection.stop();
    } catch { }
    connection = null;
  }
};

const notificarEscribiendo = async () => {
  if (connection?.state === signalR.HubConnectionState.Connected) {
    try { await connection.invoke('NotificarEscribiendo', props.ticketId); } catch { }
  }
};

const cargarMensajes = async (id) => {
  if (!id) return;
  await ticketsStore.fetchComments(id);
  messages.value = ticketsStore.comments.map(mapComment);
  scrollToBottom(false);
  nextTick(() => inputRef.value?.focus());
};

watch(() => props.ticketId, async (id) => {
  await desconectarSignalR();
  messages.value = [];
  if (id) {
    await cargarMensajes(id);
    await conectarSignalR();
  }
}, { immediate: true });

const sendMessage = async () => {
  const texto   = newMessage.value.trim();
  const interno = isInternalMode.value;
  if ((!texto && pendingFiles.value.length === 0) || !props.ticketId || sending.value) return;

  sending.value = true;
  resetInput();

  try {
    if (connection?.state === signalR.HubConnectionState.Connected && !pendingFiles.value.length) {
      await connection.invoke('EnviarMensaje', props.ticketId, texto || '📎 Archivo adjunto', interno);
    } else {
      const saved = await ticketsStore.addComment(props.ticketId, {
        mensaje: texto || '📎 Archivo adjunto',
        interno
      });
      if (pendingFiles.value.length > 0 && saved?.id) {
        await uploadFiles(saved.id);
      }
      await cargarMensajes(props.ticketId);
    }
  } catch (e) {
    console.error(e);
    newMessage.value = texto;
  } finally {
    sending.value = false;
  }
};

onMounted(() => { nextTick(() => inputRef.value?.focus()); });
onUnmounted(desconectarSignalR);
</script>

<style scoped>
.custom-scrollbar::-webkit-scrollbar { width: 4px; }
.custom-scrollbar::-webkit-scrollbar-track { background: transparent; }
.custom-scrollbar::-webkit-scrollbar-thumb { background: rgba(100,100,100,0.15); border-radius: 10px; }
</style>