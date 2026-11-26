import {phone, validateCustomer, slots, localDay, reserve, cancel} from './domain.mjs';
const $ = id => document.getElementById(id);
const key = 'banhotosa-demo-v1';
let current = null, editing = null, consulted = null;
function read() {
  const raw = localStorage.getItem(key);
  if (!raw) return {customers:[],appointments:[]};
  const state = JSON.parse(raw);
  if (!Array.isArray(state.customers) || !Array.isArray(state.appointments)) throw Error('Os dados locais estão inválidos. Use outro perfil do navegador para experimentar.');
  state.customers.forEach(validateCustomer);
  return state;
}
function write(state) { localStorage.setItem(key, JSON.stringify(state)); }
function message(text, error = false) { $('message').textContent = text; $('message').classList.toggle('error',error); }
function run(action) { try { action(); } catch(e) { message(e instanceof SyntaxError ? 'Não foi possível ler os dados locais. Nenhum dado foi sobrescrito.' : e.message, true); } }
function node(tag, text, className) { const el = document.createElement(tag); if(text) el.textContent=text; if(className) el.className=className; return el; }
function tab(name) {
  for(const section of ['agenda','cadastro']) { $(`${section}-panel`).hidden=section!==name; $(`${section}-tab`).setAttribute('aria-pressed',String(section===name)); }
}
$('agenda-tab').onclick=()=>tab('agenda'); $('cadastro-tab').onclick=()=>tab('cadastro');
$('open-cadastro').onclick=()=>tab('cadastro');
const lookup=$('lookup'), form=$('customer-form');
lookup.elements.date.value=localDay(); lookup.elements.date.min=localDay();
function invalidate() {current=null;consulted=null;$('booking').hidden=true;$('empty').hidden=false;}
lookup.oninput=invalidate;
function petRow(p={id:crypto.randomUUID(),name:'',breed:'',age:''}) {
  const row=node('div',null,'pet-row'); row.dataset.id=p.id;
  for(const [field,label,type,max] of [['name','Nome do pet','text',80],['breed','Raça','text',80],['age','Idade (anos)','number',32767]]) {
    const l=node('label',label), input=document.createElement('input'); input.dataset.field=field; input.type=type;input.required=true;input.value=p[field];
    if(type==='number'){input.min='0';input.max=String(max);input.step='1';}else input.maxLength=max;
    l.append(input);row.append(l);
  }
  const remove=node('button','Remover','secondary');remove.type='button';remove.onclick=()=>{row.remove();updatePets();};row.append(remove);$('pets').append(row);updatePets();
}
function updatePets() {const rows=[...$('pets').children];$('add-pet').disabled=rows.length>=4;rows.forEach(r=>r.querySelector('button').disabled=editing!==null || rows.length===1);}
$('add-pet').onclick=()=>{if($('pets').children.length<4)petRow();};
function newCustomer(){editing=null;form.reset();form.elements.phone.readOnly=false;$('pets').replaceChildren();petRow();}
$('new-customer').onclick=newCustomer;
function editCustomer(){run(()=>{const c=read().customers.find(c=>c.id===current);if(!c)throw Error('Consulte novamente o cliente.');editing=c.id;for(const field of ['name','phone','address','number','district','city','zip'])form.elements[field].value=c[field];form.elements.phone.readOnly=true;$('pets').replaceChildren();c.pets.forEach(petRow);tab('cadastro');});}
form.onsubmit=e=>{e.preventDefault();run(()=>{
  const state=read(), c={id:editing||crypto.randomUUID(),pets:[]};
  for(const field of ['name','phone','address','number','district','city','zip'])c[field]=form.elements[field].value;
  c.phone=phone(c.phone);c.zip=c.zip.replace('-','');
  for(const row of $('pets').children){const p={id:row.dataset.id};row.querySelectorAll('input').forEach(i=>p[i.dataset.field]=i.dataset.field==='age'?Number(i.value):i.value);c.pets.push(p);}
  validateCustomer(c);
  if(state.customers.some(x=>x.phone===c.phone&&x.id!==c.id))throw Error('Telefone já cadastrado. Consulte o cliente para editar.');
  const index=state.customers.findIndex(x=>x.id===c.id);
  if(editing&&index<0)throw Error('Cadastro não encontrado. Consulte novamente.');
  if(index<0)state.customers.push(c);else state.customers[index]=c;
  write(state);newCustomer();lookup.elements.phone.value=c.phone;invalidate();tab('agenda');message('Cadastro salvo! Agora consulte os horários para agendar.');
});};
function showAgenda() {
  const state=read(), c=state.customers.find(c=>c.phone===phone(lookup.elements.phone.value));
  if(!c)throw Error('Cliente não cadastrado. Abra “Cliente e pets” para cadastrar.');
  const day=lookup.elements.date.value, times=slots(day);
  if(day<localDay())throw Error('Escolha uma data a partir de hoje.');
  current=c.id;consulted={phone:c.phone,day};
  $('customer-name').replaceChildren(node('span',`Olá, ${c.name}.`));const edit=node('button','Editar cadastro','secondary');edit.type='button';edit.onclick=editCustomer;$('customer-name').append(edit);
  const select=$('reserve').elements.pet;select.replaceChildren();c.pets.forEach(p=>{const option=node('option',p.name);option.value=p.id;select.append(option);});
  $('slots').replaceChildren();let available=0;
  for(const start of times){const occupied=state.appointments.some(a=>a.start===start), past=new Date(start)<=new Date();
    const label=node('label',null,'slot'), radio=document.createElement('input');radio.type='radio';radio.name='slot';radio.value=start;radio.required=true;radio.disabled=occupied||past;
    const caption=node('span',start.slice(11));caption.append(node('small',past?'Encerrado':occupied?'Ocupado':'Disponível'));label.append(radio,caption);$('slots').append(label);if(!radio.disabled)available++;
  }
  if(!times.length)$('slots').append(node('p','Não há atendimento aos domingos.'));
  $('book-button').disabled=available===0;
  $('appointments').replaceChildren();
  for(const a of state.appointments.filter(a=>a.customerId===c.id&&a.start.startsWith(day)).sort((a,b)=>a.start.localeCompare(b.start))){
    const row=node('div',null,'appointment');row.append(node('p',`${a.start.slice(11)} · ${c.pets.find(p=>p.id===a.petId)?.name||'Pet'} · ${a.service}`));
    const button=node('button','Cancelar','secondary');button.type='button';button.disabled=new Date(a.start)<=new Date();button.onclick=()=>{if(!confirm(`Cancelar o agendamento das ${a.start.slice(11)}?`))return;run(()=>{const latest=read();latest.appointments=cancel(latest,a.id,c.id);write(latest);showAgenda();message('Agendamento cancelado. Horário liberado.');});};row.append(button);$('appointments').append(row);
  }
  if(!$('appointments').children.length)$('appointments').append(node('p','Nenhum agendamento deste cliente nesta data.'));
  $('booking').hidden=false;$('empty').hidden=true;
}
lookup.onsubmit=e=>{e.preventDefault();run(()=>{showAgenda();message('Disponibilidade atualizada. Escolha o pet, o cuidado e o horário.');});};
$('reserve').onsubmit=e=>{e.preventDefault();run(()=>{
  if(!consulted||consulted.phone!==phone(lookup.elements.phone.value)||consulted.day!==lookup.elements.date.value)throw Error('Consulte novamente os horários.');
  const data=new FormData(e.target),state=read();state.appointments.push(reserve(state,current,data.get('pet'),data.get('slot')||'',data.get('service')));write(state);showAgenda();message('Agendamento confirmado nesta demonstração!');
});};
window.addEventListener('storage',e=>{if(e.key===key){invalidate();message('Os dados foram alterados em outra aba. Consulte novamente.');}});
newCustomer();
run(()=>{read();});
