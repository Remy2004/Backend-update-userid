import { useState } from 'react';
import axios from 'axios';
import { UserPlus } from 'lucide-react';

export default function AdminsManagement() {
    const [form, setForm] = useState({ fullName: '', email: '', password: '' });
    const [msg, setMsg] = useState({ type: '', text: '' });

    const handleSubmit = async (e) => {
        e.preventDefault();
        setMsg({ type: '', text: 'جاري الإضافة...' });
        try {
            const token = localStorage.getItem('admin_token');
            await axios.post('http://localhost:5055/api/AdminDashboard/add-admin', form, { headers: { Authorization: `Bearer ${token}` } });
            setMsg({ type: 'success', text: 'تمت إضافة المدير بنجاح!' });
            setForm({ fullName: '', email: '', password: '' });
        } catch (err) {
            setMsg({ type: 'error', text: err.response?.data?.message || 'حدث خطأ أثناء الإضافة' });
        }
    };

    return (
        <div className="max-w-xl mx-auto bg-white p-8 rounded-xl shadow-sm">
            <div className="flex items-center gap-3 mb-6 border-b pb-4">
                <UserPlus className="text-teal-600 w-8 h-8" />
                <h3 className="text-xl font-bold">إضافة مدير نظام جديد</h3>
            </div>
            {msg.text && <div className={`p-3 mb-4 rounded-lg font-bold ${msg.type === 'success' ? 'bg-green-100 text-green-700' : 'bg-red-100 text-red-700'}`}>{msg.text}</div>}
            <form onSubmit={handleSubmit} className="space-y-4">
                <div><label className="block text-sm font-bold mb-1">الاسم الكامل</label><input type="text" required value={form.fullName} onChange={e => setForm({...form, fullName: e.target.value})} className="w-full p-2 border rounded-lg focus:ring-teal-500" /></div>
                <div><label className="block text-sm font-bold mb-1">البريد الإلكتروني</label><input type="email" required value={form.email} onChange={e => setForm({...form, email: e.target.value})} className="w-full p-2 border rounded-lg focus:ring-teal-500 text-left" dir="ltr" /></div>
                <div><label className="block text-sm font-bold mb-1">كلمة المرور (يجب أن تحتوي على حروف كبيرة ورموز)</label><input type="password" required value={form.password} onChange={e => setForm({...form, password: e.target.value})} className="w-full p-2 border rounded-lg focus:ring-teal-500 text-left" dir="ltr" /></div>
                <button type="submit" className="w-full bg-gray-900 text-white font-bold p-3 rounded-lg hover:bg-gray-800 transition">إضافة الصلاحية</button>
            </form>
        </div>
    );
}