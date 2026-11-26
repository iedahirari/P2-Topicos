export const services = ['Banho', 'Tosa', 'Banho e tosa'];
export function phone(value) {
  if (!/^[\d ()-]+$/.test(value)) throw Error('Informe um telefone válido com DDD.');
  const digits = value.replace(/\D/g, '');
  if (!/^\d{10,11}$/.test(digits)) throw Error('Informe o telefone com DDD, com 10 ou 11 dígitos.');
  return digits;
}
export function text(value, limit, label) {
  value = String(value ?? '').trim();
  if (!value || value.length > limit || /[\x00-\x1f\x7f]/.test(value)) throw Error(`${label}: informe de 1 a ${limit} caracteres.`);
  return value;
}
export function validateCustomer(c) {
  phone(c.phone);
  for (const [key, limit, label] of [['name',120,'Nome'],['address',200,'Endereço'],['number',20,'Número'],['district',80,'Bairro'],['city',80,'Cidade']]) c[key] = text(c[key],limit,label);
  if (!/^\d{8}$/.test(c.zip)) throw Error('Informe um CEP com 8 dígitos.');
  if (!Array.isArray(c.pets) || c.pets.length < 1 || c.pets.length > 4) throw Error('Cadastre de um a quatro pets.');
  for (const p of c.pets) {
    p.name = text(p.name,80,'Nome do pet'); p.breed = text(p.breed,80,'Raça');
    if (!Number.isInteger(p.age) || p.age < 0 || p.age > 32767) throw Error('Informe uma idade inteira não negativa para o pet.');
  }
  return c;
}
export function slots(day) {
  if (!/^\d{4}-\d{2}-\d{2}$/.test(day)) throw Error('Informe uma data válida.');
  const d = new Date(`${day}T12:00:00`);
  if (Number.isNaN(+d) || localDay(d) !== day) throw Error('Informe uma data válida.');
  return d.getDay() === 0 ? [] : Array.from({length:8},(_,i)=>`${day}T${String(i+9).padStart(2,'0')}:00`);
}
export function localDay(date = new Date()) {
  return `${date.getFullYear()}-${String(date.getMonth()+1).padStart(2,'0')}-${String(date.getDate()).padStart(2,'0')}`;
}
export function reserve(state, customerId, petId, start, service, now = new Date()) {
  const c = state.customers.find(c=>c.id === customerId);
  if (!c || !c.pets.some(p=>p.id === petId)) throw Error('Consulte o cliente e selecione um pet do cadastro.');
  if (!services.includes(service)) throw Error('Selecione um serviço válido.');
  if (!slots(start.slice(0,10)).includes(start) || new Date(start) <= now) throw Error('Escolha um horário futuro de segunda a sábado, das 9h às 16h.');
  if (state.appointments.some(a=>a.start === start)) throw Error('Este horário já está ocupado. Consulte novamente.');
  return {id:crypto.randomUUID(), customerId, petId, start, service};
}
export function cancel(state, id, customerId, now = new Date()) {
  const a = state.appointments.find(a=>a.id === id);
  if (!a || a.customerId !== customerId) throw Error('Selecione um agendamento deste cliente.');
  if (new Date(a.start) <= now) throw Error('Só é possível cancelar agendamentos futuros.');
  return state.appointments.filter(a=>a.id !== id);
}
